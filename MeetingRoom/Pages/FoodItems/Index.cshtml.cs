using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using MeetingRoom.Data;
using MeetingRoom.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingRoom.Pages.FoodItems
{
    public class Index : PageModel
    {
        private readonly IMediator _mediator;

        public Result Data { get; private set; }

        public Index(IMediator mediator) => _mediator = mediator;

        public async Task OnGetAsync(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
            => Data = await _mediator.Send(new Query { CurrentFilter = currentFilter, Page = pageIndex, SearchString = searchString, SortOrder = sortOrder });

        public class Query : IRequest<Result>
        {
            public string SortOrder { get; set; }
            public string CurrentFilter { get; set; }
            public string SearchString { get; set; }
            public int? Page { get; set; }
        }

        public class Result
        {
            public string CurrentSort { get; set; }
            public string NameSortParm { get; set; }
            public string DateSortParm { get; set; }
            public string CurrentFilter { get; set; }
            public string SearchString { get; set; }
            public PaginatedList<Model> Results { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Food, Result>();
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly ExamContext _db;
            private readonly IConfigurationProvider _configuration;

            public Handler(ExamContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var model = new Result
                {
                    CurrentSort = request.SearchString,
                    NameSortParm = String.IsNullOrEmpty(request.SortOrder) ? "name_desc" : ""
                };

                if (request.SearchString != null)
                {
                    request.Page = 1;
                }
                else
                {
                    request.SearchString = request.CurrentFilter;
                }

                model.CurrentFilter = request.SearchString;
                model.SearchString = request.SearchString;

                IQueryable<Food> foodItems = _db.FoodItems;


                if (!String.IsNullOrEmpty(request.SearchString))
                {
                    foodItems = foodItems.Where(r => r.Name.Contains(request.SearchString));
                }

                int pageSize = 15;
                int pageNumber = (request.Page ?? 1);

                model.Results = await foodItems
                    .ProjectTo<Model>(_configuration)
                    .PaginatedListAsync(pageNumber, pageSize);

                return model;
            }
        }
    }
}