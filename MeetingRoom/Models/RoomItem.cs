namespace MeetingRoom.Models
{
    public class RoomItem
    {
        public int RoomId { get; set; }
        public int RoomAttributeId { get; set; }
        public Room Room { get; set; }
        public RoomAttribute RoomAttribute { get; set; }
    }
}
