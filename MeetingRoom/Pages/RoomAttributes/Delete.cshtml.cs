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

namespace MeetingRoom.Pages.RoomAttributes
{
    public class Delete : PageModel
    {
        private readonly IMediator _mediator;

        [BindProperty]
        public Command Data { get; set; }

        public Delete(IMediator mediator) => _mediator = mediator;

        public async Task OnGetAsync(Query query) => Data = await _mediator.Send(query);

        public async Task<IActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);

            return this.RedirectToPageJson(nameof(Index));
        }

        public class Query : IRequest<Command>
        {
            public int Id { get; set; }
        }

        public class Command : IRequest
        {
            public Command()
            {
                AffectedRooms = new List<Room>();
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public List<Room> AffectedRooms { get; set; }

            public class Room
            {
                public string Name { get; set; }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<RoomAttribute, Command>();
                CreateMap<Room, Command.Room>();
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
                    .RoomAttributes
                    .Where(r => r.Id == request.Id)
                    .ProjectTo<Command>(_configuration)
                    .SingleOrDefaultAsync(cancellationToken);
                model.AffectedRooms = await _db
                    .RoomItems
                    .Where(r => r.RoomAttributeId == model.Id)
                    .Select(select => select.Room)
                    .ProjectToListAsync<Command.Room>(_configuration);

                return model;
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly ExamContext _db;

            public CommandHandler(ExamContext db) => _db = db;

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var roomAttribute = await _db.RoomAttributes.FindAsync(request.Id);
                var items = await _db.RoomItems.Where(r => r.RoomAttributeId == roomAttribute.Id).ToListAsync();

                foreach (var item in items)
                {
                    _db.RoomItems.Remove(item);
                }

                _db.RoomAttributes.Remove(roomAttribute);

                return default;
            }
        }
    }
}