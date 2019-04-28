using MeetingRoom.Models;
using MeetingRoom.Pages.Rooms;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static MeetingRoom.IntegrationTests.SliceFixture;

namespace MeetingRoom.IntegrationTests.Features.Rooms
{
    public class IndexTests : IntegrationTestBase
    {
        [Fact]
        public async Task Should_return_all_items_for_default_search()
        {
            var room1 = new Room
            {
                Name = "Room 1"
            };
            var room2 = new Room
            {
                Name = "Room 2"
            };

            await InsertAsync(room1, room2);

            var query = new Index.Query { CurrentFilter = "Room" };

            var result = await SendAsync(query);

            result.Results.Count.ShouldBeGreaterThanOrEqualTo(1);
            result.Results.Select(r => r.Id).ShouldContain(room1.Id);
            result.Results.Select(r => r.Id).ShouldContain(room2.Id);
        }
    }
}
