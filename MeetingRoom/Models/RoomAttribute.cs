namespace MeetingRoom.Models
{
    public class RoomAttribute : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
