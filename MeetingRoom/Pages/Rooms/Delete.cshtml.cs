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
    public class Delete : PageModel
    {
        private readonly IMediator _mediator;

        public Delete(IMediator mediator) => _mediator = mediator;

        [BindProperty]
        public Command Data { get; set; }

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
                Items = new List<Item>();
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public List<Item> Items { get; set; }

            public class Item
            {
                public string Name { get; set; }
                public string Value { get; set; }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Room, Command>();
                CreateMap<RoomAttribute, Command.Item>();
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
                var room = await _db
                    .Rooms
                    .Where(r => r.Id == request.Id)
                    .ProjectTo<Command>(_configuration)
                    .SingleOrDefaultAsync(cancellationToken);
                room.Items = await _db
                    .RoomItems
                    .Where(r => r.RoomId == room.Id)
                    .Select(select => select.RoomAttribute)
                    .ProjectToListAsync<Command.Item>(_configuration);

                return room;
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly ExamContext _db;

            public CommandHandler(ExamContext db) => _db = db;

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var room = await _db.Rooms.FindAsync(request.Id);
                var items = await _db.RoomItems.Where(r => r.RoomId == room.Id).ToListAsync();

                foreach (var item in items)
                {
                    _db.RoomItems.Remove(item);
                }

                _db.Rooms.Remove(room);

                return default;
            }
        }
    }
}