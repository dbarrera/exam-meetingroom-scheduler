using AutoMapper;
using MediatR;
using MeetingRoom.Data;
using MeetingRoom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingRoom.Pages.Rooms
{
    public class Create : PageModel
    {
        private readonly IMediator _mediator;

        public Create(IMediator mediator) => _mediator = mediator;

        [BindProperty]
        public Command Data { get; set; }

        public async Task OnGetAsync() => Data = await _mediator.Send(new Query());

        public async Task<IActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);

            return this.RedirectToPageJson(nameof(Index));
        }

        public class Query : IRequest<Command>
        {
        }

        public class Command : IRequest<int>
        {
            public Command()
            {
                SelectedAttributes = new int[0];
                AvailableAttributes = new List<Attribute>();
            }

            public string Name { get; set; }
            [IgnoreMap]
            public int[] SelectedAttributes { get; set; }
            public List<Attribute> AvailableAttributes { get; set; }

            public class Attribute
            {
                public int Id { get; set; }
                public string DisplayValue { get; set; }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Command, Room>(MemberList.Source);
                CreateMap<Command.Attribute, RoomAttribute>();
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
                => new Command { AvailableAttributes = await _db.RoomAttributes.ProjectToListAsync<Command.Attribute>(_configuration) };
        }

        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly ExamContext _db;
            private readonly IMapper _mapper;

            public CommandHandler(ExamContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var room = _mapper.Map<Command, Room>(request);

                _db.Rooms.Add(room);

                for (int i = 0; i < request.SelectedAttributes.Length; i++)
                {
                    var roomItem = new RoomItem
                    {
                        RoomId = room.Id,
                        RoomAttributeId = request.SelectedAttributes[i]
                    };

                    _db.RoomItems.Add(roomItem);
                }

                await _db.SaveChangesAsync(cancellationToken);

                return room.Id;
            }
        }
    }
}