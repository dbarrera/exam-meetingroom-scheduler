using AutoMapper;
using MediatR;
using MeetingRoom.Data;
using MeetingRoom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public class Command : IRequest<int>
        {
            public string Name { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Command, Room>(MemberList.Source);
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