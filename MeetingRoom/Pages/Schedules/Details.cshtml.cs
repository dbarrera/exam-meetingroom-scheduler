using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using MeetingRoom.Data;
using MeetingRoom.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingRoom.Pages.Schedules
{
    public class Details : PageModel
    {
        private readonly IMediator _mediator;

        public Model Data { get; private set; }

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
            public string Room { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }

            [IgnoreMap]
            public string Date
                => StartTime.Date == EndTime.Date
                   ? StartTime.Date.ToString("dd/MM/yyyy")
                   : $"{StartTime.Date.ToString("dd/MM/yyyy")} - {EndTime.Date.ToString("dd/MM/yyyy")}";

            [IgnoreMap]
            public string Duration
                => $"{StartTime.ToString("hh:mm tt")} - {EndTime.ToString("hh:mm tt")}";
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Schedule, Model>()
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room.Name));
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly ExamContext _db;
            private readonly IConfigurationProvider _configuration;

            public Handler(ExamContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
                => await _db.Schedules
                    .Where(s => s.Id == request.Id)
                    .Include(s => s.Room)
                    .ProjectTo<Model>(_configuration)
                    .SingleOrDefaultAsync(cancellationToken);
        }
    }
}