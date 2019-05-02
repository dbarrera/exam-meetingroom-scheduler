using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using MeetingRoom.Data;
using MeetingRoom.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingRoom.Pages.Schedules
{
    public class Index : PageModel
    {
        private readonly IMediator _mediator;

        public Model Data { get; private set; }

        public Index(IMediator mediator) => _mediator = mediator;

        public async Task OnGetAsync(Query query)
            => Data = await _mediator.Send(query);

        public class Query : IRequest<Model>
        {
            public int? Id { get; set; }
            public int? FoodId { get; set; }
        }

        public class Model
        {
            public int? Id { get; set; }

            public IList<Schedule> Schedules;
            public IList<Serving> ServingInfo;

            public class Schedule
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public DateTime StartTime { private get; set; }
                public DateTime EndTime { private get; set; }
                public List<Serving> Servings { get; set; }

                [IgnoreMap]
                public string Date
                    => StartTime.Date == EndTime.Date
                       ? StartTime.Date.ToString("dd/MM/yyyy")
                       : $"{StartTime.Date.ToString("dd/MM/yyyy")} - {EndTime.Date.ToString("dd/MM/yyyy")}";

                [IgnoreMap]
                public string Duration
                    => $"{StartTime.ToString("hh:mm tt")} - {EndTime.ToString("hh:mm tt")}";
            }

            public class Serving
            {
                public string Name { get; set; }
                public DateTime TimeIn { get; set; }

                [IgnoreMap]
                public string DisplayTime
                    => TimeIn.ToString("hh:mm tt");
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Schedule, Model.Schedule>();
                CreateMap<ServingSchedule, Model.Serving>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Food.Name));
            }
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
            {
                var schedules = await _db.Schedules
                    .Include(s => s.Servings)
                    .ThenInclude(s => s.Food)
                    .ProjectToListAsync<Model.Schedule>(_configuration);

                var servings = new List<Model.Serving>();

                if (request.Id != null)
                {
                    servings = await _db.ServingSchedules
                        .Where(s => s.ScheduleId == request.Id)
                        .ProjectTo<Model.Serving>(_configuration)
                        .ToListAsync(cancellationToken);
                }

                var viewModel = new Model
                {
                    Id = request.Id,
                    Schedules = schedules,
                    ServingInfo = servings
                };

                return viewModel;
            }
        }
    }
}