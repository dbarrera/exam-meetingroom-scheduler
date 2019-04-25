using System.Collections.Generic;

namespace MeetingRoom.Models
{
    public class Room : IEntity
    {
        public Room()
        {
            //Attributes = new List<RoomAttribute>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        //public IList<RoomAttribute> Attributes { get; set; }
    }
}
