using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingRoom.Models
{
    public class Schedule : IEntity
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string Name { get; set; }
        [Column(TypeName="datetime2")]
        public DateTime StartTime { get; set; }
        [Column(TypeName="datetime2")]
        public DateTime EndTime { get; set; }
        public Room Room { get; set; }
        public ICollection<ServingSchedule> Servings { get; private set; } = new List<ServingSchedule>();
    }
}
