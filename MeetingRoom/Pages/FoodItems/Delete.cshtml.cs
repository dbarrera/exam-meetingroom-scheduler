using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using MeetingRoom.Data;
using MeetingRoom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingRoom.Pages.FoodItems
{
    public class Delete : PageModel
    {
        private readonly IMediator _mediator;

        [BindProperty]
        public Command Data { get; set; }

        public async Task OnGetAsync(Query query) => Data = await _mediator.Send(query);

        public Delete(IMediator mediator) => _mediator = mediator;

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
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Food, Command>();
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
                => await _db
                .FoodItems
                .Where(f => f.Id == request.Id)
                .ProjectTo<Command>(_configuration)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly ExamContext _db;

            public CommandHandler(ExamContext db) => _db = db;

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var food = await _db.FoodItems.FindAsync(request.Id);

                _db.FoodItems.Remove(food);

                return default;
            }
        }
    }
}