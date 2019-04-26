using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using MeetingRoom.Data;
using MeetingRoom.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingRoom.Pages.Rooms
{
    public class Details : PageModel
    {
        private readonly IMediator _mediator;

        public Details(IMediator mediator) => _mediator = mediator;

        public Model Data { get; private set; }

        public async Task OnGetAsync(Query query)
            => Data = await _mediator.Send(query);

        public class Query : IRequest<Model>
        {
            public int Id { get; set; }
        }

        public class Model
        {
            public Model()
            {
                Items = new List<Item>();
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public IList<Item> Items { get; set; }

            public class Item
            {
                public string Name { get; set; }
                public string Value { get; set; }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Room, Model>();
                CreateMap<RoomAttribute, Model.Item>();
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
                var room = await _db
                    .Rooms
                    .Where(r => r.Id == request.Id)
                    .ProjectTo<Model>(_configuration)
                    .SingleOrDefaultAsync(cancellationToken);
                room.Items = await _db
                    .RoomItems
                    .Where(r => r.RoomId == room.Id)
                    .ProjectToListAsync<Model.Item>(_configuration);

                return room;
            }
        }
    }
}