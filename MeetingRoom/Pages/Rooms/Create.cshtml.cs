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

        public void OnGet() => Data = new Command();

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
                Attributes = new List<Attribute>();
            }

            public string Name { get; set; }
            public List<Attribute> Attributes { get; set; }

            public class Attribute
            {
                public int Id { get; set; }
                public string DisplayValue { get; set; }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Command, Room>(MemberList.Source);
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
                //var model = await _db.;
            }
        }

        public class Handler : IRequestHandler<Command, int>
        {
            private readonly ExamContext _db;
            private readonly IMapper _mapper;

            public Handler(ExamContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var room = _mapper.Map<Command, Room>(request);

                _db.Rooms.Add(room);

                await _db.SaveChangesAsync(cancellationToken);

                return room.Id;
            }
        }
    }
}