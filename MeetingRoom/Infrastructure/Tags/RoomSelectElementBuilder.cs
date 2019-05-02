using MeetingRoom.Models;

namespace MeetingRoom.Infrastructure.Tags
{
    public class RoomSelectElementBuilder : EntitySelectElementBuilder<Room>
    {
        protected override int GetValue(Room instance)
        {
            return instance.Id;
        }

        protected override string GetDisplayValue(Room instance)
        {
            return instance.Name;
        }
    }
}
