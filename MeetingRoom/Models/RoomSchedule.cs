using System;

namespace MeetingRoom.Models
{
    public class RoomSchedule : IEntity
    {
        public int Id => Id;

        public DateTime Date { get; set; }
        public DateTime Start { get; set; } 
        public DateTime End { get; set; }
        public Room Room { get; set; }
    }
}
