using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using MeetingRoom.Data;
using MeetingRoom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingRoom.Pages.Rooms
{
    public class Edit : PageModel
    {
        private readonly IMediator _mediator;

        [BindProperty]
        public Command Data { get; set; }

        public Edit(IMediator mediator) => _mediator = mediator;

        public async Task OnGetAsync(Query query) 
            => Data = await _mediator.Send(query);

        public async Task<IActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);

            return this.RedirectToPageJson(nameof(Index));
        }

        public class Query : IRequest<Command>
        {
            public int Id { get; set; }
        }

        public class Command : IRequest<int>
        {
            public Command()
            {
                SelectedAttributes = new int[0];
                AssignedItems = new List<Item>();
            }

            public int Id { get; set; }
            public string Name { get; set; }
            [IgnoreMap]
            public int[] SelectedAttributes { get; set; }
            [IgnoreMap]
            public List<Item> AssignedItems { get; set; }

            public class Item
            {
                public int AttributeId { get; set; }
                public string DisplayValue { get; set; }
                public bool Assigned { get; set; }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Command, Room>(MemberList.Source);
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ExamContext _db;
            private readonly IConfigurationProvider _configuration;

            public QueryHandler(ExamContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public async Task<Command> Handle(Query request, CancellationToken cancellationToken)
            {
                var model = await _db
                    .Rooms
                    .Where(r => r.Id == request.Id)
                    .ProjectTo<Command>(_configuration)
                    .SingleOrDefaultAsync(cancellationToken);

                PopulateAssignedItems(model);

                return model;
            }

            private void PopulateAssignedItems(Command model)
            {
                var allAttributes = _db.RoomAttributes;
                var assignedItems = 
                    new HashSet<int>(_db.RoomItems.Where(a => a.RoomId == model.Id).Select(select => select.RoomAttributeId));
                var viewModels = allAttributes.Select(item => new Command.Item
                {
                    AttributeId = item.Id,
                    DisplayValue = item.DisplayValue,
                    Assigned = assignedItems.Any() && assignedItems.Contains(item.Id)
                }).ToList();
                model.AssignedItems = viewModels;
            }
        }

        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly ExamContext _db;

            public CommandHandler(ExamContext db) => _db = db;

            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                // Update room
                var room = await _db
                    .Rooms
                    .Where(r => r.Id == request.Id)
                    .SingleAsync(cancellationToken);
                room.Name = request.Name;

                // Update room items
                var assignedRoomItems = await _db
                    .RoomItems
                    .Where(r => r.RoomId == room.Id)
                    .ToListAsync(cancellationToken);

                var selectedAttributesHS =
                    new HashSet<int>(request.SelectedAttributes);
                var assignedAttributesHS = new HashSet<int>
                    (assignedRoomItems.Select(a => a.RoomAttributeId));

                foreach (var attribute in await _db.RoomAttributes.ToListAsync(cancellationToken))
                {
                    if (selectedAttributesHS.Contains(attribute.Id))
                    {
                        if (!assignedAttributesHS.Contains(attribute.Id))
                        {
                            _db.RoomItems.Add(new RoomItem
                            {
                                Room = room,
                                RoomAttribute = attribute
                            });
                        }
                    }
                    else
                    {
                        if (assignedAttributesHS.Contains(attribute.Id))
                        {
                            var toRemove = assignedRoomItems.Single(a => a.RoomAttributeId == attribute.Id);
                            _db.RoomItems.Remove(toRemove);
                        }
                    }
                }

                await _db.SaveChangesAsync(cancellationToken);

                return room.Id;
            }
        }
    }
}