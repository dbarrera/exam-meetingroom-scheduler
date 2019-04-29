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

        [Fact]
        public async Task Should_create_room_with_items()
        {
            var attribute1 = new RoomAttribute
            {
                Name = "Size", Value = "400 Square Ft"
            };
            var attribute2 = new RoomAttribute
            {
                Name = "Windows", Value = "5"
            };
            var attribute3 = new RoomAttribute
            {
                Name = "Entrance Door", Value = "1"
            };
            var attribute4 = new RoomAttribute
            {
                Name = "Projector", Value = ""
            };
            await InsertAsync(attribute1, attribute2, attribute3, attribute4);

            var cmd = new Create.Command
            {
                Name = "Room 1",
                SelectedAttributes = new int[] { attribute1.Id, attribute2.Id, attribute3.Id, attribute4.Id }
            };

            int roomId = await SendAsync(cmd);
            var room = await FindAsync<Room>(roomId);

            room.ShouldNotBeNull();
            room.Name.ShouldBe(cmd.Name);
        }
    }
}
