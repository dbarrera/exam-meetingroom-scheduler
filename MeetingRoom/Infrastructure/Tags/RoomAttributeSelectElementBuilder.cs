using MeetingRoom.Models;

namespace MeetingRoom.Infrastructure.Tags
{
    public class RoomAttributeSelectElementBuilder : EntitySelectElementBuilder<RoomAttribute>
    {
        protected override int GetValue(RoomAttribute instance)
        {
            return instance.Id;
        }

        protected override string GetDisplayValue(RoomAttribute instance)
        {
            return instance.DisplayValue;
        }
    }
}
