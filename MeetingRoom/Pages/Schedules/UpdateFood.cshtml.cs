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
    public class UpdateFood : PageModel
    {
        private readonly IMediator _mediator;

        [BindProperty]
        public Command Data { get; set; }

        public UpdateFood(IMediator mediator) => _mediator = mediator;

        public async Task OnGetAsync(Query query) 
            => Data = await _mediator.Send(query);

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
                AssignedFood = new List<FoodData>();
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime StartTime { private get; set; }
            public DateTime EndTime { private get; set; }

            [IgnoreMap]
            public List<FoodData> AssignedFood { get; set; }

            [IgnoreMap]
            public string Date
                => StartTime.Date == EndTime.Date
                   ? StartTime.Date.ToString("dd/MM/yyyy")
                   : $"{StartTime.Date.ToString("dd/MM/yyyy")} - {EndTime.Date.ToString("dd/MM/yyyy")}";

            [IgnoreMap]
            public string Duration
                => $"{StartTime.ToString("hh:mm tt")} - {EndTime.ToString("hh:mm tt")}";

            public class FoodData
            {
                public int FoodId { get; set; }
                public string Name { get; set; }
                public DateTime TimeIn { get; set; }
                public bool Assigned { get; set; }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Command, Schedule>(); 
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
                var model = await _db.Schedules
                    .Where(s => s.Id == request.Id)
                    .ProjectTo<Command>(_configuration)
                    .SingleOrDefaultAsync(cancellationToken);

                PopulateAssignedFood(model);

                return model;
            }

            private void PopulateAssignedFood(Command model)
            {
                var allFood = _db.FoodItems;
                var servingSchedules = _db.ServingSchedules
                    .Where(s => s.ScheduleId == model.Id)
                    .ToList();
                var assignedFood =
                    new HashSet<int>(servingSchedules.Select(s => s.FoodId));

                foreach (var food in allFood)
                {
                    var viewModel = new Command.FoodData
                    {
                        FoodId = food.Id,
                        Name = food.Name,
                        Assigned = false
                    };
                    if (assignedFood.Any() && assignedFood.Contains(food.Id))
                    {
                        viewModel.TimeIn = servingSchedules.Single(s => s.FoodId == food.Id).TimeIn;
                        viewModel.Assigned = true;
                    }

                    model.AssignedFood.Add(viewModel);
                }
            }
        }

        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly ExamContext _db;

            public CommandHandler(ExamContext db) => _db = db;

            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var schedule = await _db.Schedules
                    .Where(s => s.Id == request.Id)
                    .Include(s => s.Servings)
                    .SingleAsync(cancellationToken);

                var servings = schedule.Servings;
                var allFood = await _db.FoodItems.ToListAsync();
                var assignedFoodHS = new HashSet<int>
                    (schedule.Servings.Select(s => s.FoodId));
                var selectedFoodHS = new HashSet<int>
                    (request.AssignedFood.Where(a => a.Assigned).Select(s => s.FoodId));

                foreach (var food in allFood)
                {
                    if (selectedFoodHS.Contains(food.Id))
                    {
                        var timeIn = request.AssignedFood.Single(a => a.FoodId == food.Id).TimeIn;

                        if (!assignedFoodHS.Contains(food.Id))
                        {
                            _db.ServingSchedules.Add(new ServingSchedule
                            {
                                Food = food,
                                Schedule = schedule,
                                TimeIn = timeIn
                            });
                        }
                        else
                        {
                            var updateTimeIn = servings.Single(s => s.FoodId == food.Id);
                            updateTimeIn.TimeIn = timeIn;
                        }
                    }
                    else
                    {
                        if (assignedFoodHS.Contains(food.Id))
                        {
                            var toREmove = servings.Single(s => s.FoodId == food.Id);
                            _db.ServingSchedules.Remove(toREmove);
                        }
                    }
                }

                await _db.SaveChangesAsync(cancellationToken);

                return schedule.Id;
            }
        }
    }
}