using System.Collections.Generic;

namespace MeetingRoom.Models
{
    public class Room : IEntity
    {
        //public Room()
        //{
        //    Items = new List<RoomItem>();
        //}

        public int Id { get; set; }
        public string Name { get; set; }

        // A change of design.
        // Lets make room not aware of how many
        // attribute it has.
        //public IList<RoomItem> Items { get; set; }
    }
}
