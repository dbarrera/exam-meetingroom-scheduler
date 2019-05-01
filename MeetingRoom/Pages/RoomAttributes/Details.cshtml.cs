using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using MeetingRoom.Data;
using MeetingRoom.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingRoom.Pages.RoomAttributes
{
    public class Details : PageModel
    {
        private readonly IMediator _mediator;

        public Model Data { get; set; }

        public Details(IMediator mediator) => _mediator = mediator;

        public async Task OnGetAsync(Query query)
            => Data = await _mediator.Send(query);

        public class Query : IRequest<Model>
        {
            public int Id { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<RoomAttribute, Model>();
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly ExamContext _context;
            private readonly IConfigurationProvider _configuration;

            public Handler(ExamContext context, IConfigurationProvider configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public Task<Model> Handle(Query request, CancellationToken cancellationToken)
                => _context.RoomAttributes
                .FromSql(@"SELECT * FROM RoomAttribute WHERE ID = {0}", request.Id)
                .ProjectTo<Model>(_configuration)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}