using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using MeetingRoom.Data;
using MeetingRoom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Pages.Schedules
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
            public MappingProfile() => CreateMap<Schedule, Command>()
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room.Name));
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
                => await _db.Schedules
                .Where(s => s.Id == request.Id)
                .Include(s => s.Room)
                .ProjectTo<Command>(_configuration)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly ExamContext _db;

            public CommandHandler(ExamContext db) => _db = db;

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var schedule = await _db.Schedules.FindAsync(request.Id);
                var servingSchedules = await _db.ServingSchedules.Where(r => r.ScheduleId == schedule.Id).ToListAsync();

                foreach (var servingSchedule in servingSchedules)
                {
                    _db.ServingSchedules.Remove(servingSchedule);
                }

                _db.Schedules.Remove(schedule);

                return default;
            }
        }
    }
}