using System;

namespace MeetingRoom.Models
{
    public class ServingSchedule
    {
        public int FoodId { get; set; }
        public int ScheduleId { get; set; }
        public Food Food { get; set; }
        public Schedule Schedule { get; set; }
        public DateTime TimeIn { get; set; } 
    }
}
