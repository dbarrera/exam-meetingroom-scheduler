using MeetingRoom.Models;
using MeetingRoom.Pages.Rooms;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using static MeetingRoom.IntegrationTests.SliceFixture;

namespace MeetingRoom.IntegrationTests.Features.Rooms
{
    public class DetailsTests : IntegrationTestBase
    {
        [Fact]
        public async Task Should_get_details()
        {
            var room = new Room
            {
                Name = "Room 1"
            };
            await InsertAsync(room);

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

            var roomItem1 = new RoomItem
            {
                RoomId = room.Id, RoomAttributeId = attribute1.Id
            };
            var roomItem2 = new RoomItem
            {
                RoomId = room.Id, RoomAttributeId = attribute2.Id
            };
            var roomItem3 = new RoomItem
            {
                RoomId = room.Id, RoomAttributeId = attribute3.Id
            };
            var roomItem4 = new RoomItem
            {
                RoomId = room.Id, RoomAttributeId = attribute4.Id
            };
            await InsertAsync(roomItem1, roomItem2, roomItem3, roomItem4);

            var details = await SendAsync(new Details.Query { Id = room.Id });

            details.ShouldNotBeNull();
            details.Name.ShouldBe(room.Name);
            details.Items.Count.ShouldBe(4);
        }
    }
}
