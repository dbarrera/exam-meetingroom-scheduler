using AutoMapper;
using MediatR;
using MeetingRoom.Data;
using MeetingRoom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingRoom.Pages.RoomAttributes
{
    public class Create : PageModel
    {
        private readonly IMediator _mediator;

        [BindProperty]
        public Command Data { get; set; }

        public Create(IMediator mediator) => _mediator = mediator;

        public async Task<ActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);

            return this.RedirectToPageJson("Index");
        }

        public class Command : IRequest<int>
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Command, RoomAttribute>(MemberList.Source);
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
                var roomAttribute = _mapper.Map<Command, RoomAttribute>(request);

                _db.RoomAttributes.Add(roomAttribute);

                await _db.SaveChangesAsync(cancellationToken);

                return roomAttribute.Id;
            }
        }
    }
}