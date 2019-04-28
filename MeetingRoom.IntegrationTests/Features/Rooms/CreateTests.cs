using MeetingRoom.Models;
using MeetingRoom.Pages.Rooms;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using static MeetingRoom.IntegrationTests.SliceFixture;

namespace MeetingRoom.IntegrationTests.Features.Rooms
{
    public class CreateTests : IntegrationTestBase
    {
        [Fact]
        public async Task Should_create_room()
        {
            var cmd = new Create.Command
            {
                Name = "Room 1"
            };

            var roomId = await SendAsync(cmd);

            var room = await FindAsync<Room>(roomId);

            room.ShouldNotBeNull();
            room.Name.ShouldBe(cmd.Name);
        }
    }
}
