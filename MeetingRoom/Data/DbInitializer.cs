using MeetingRoom.Models;
using System.Linq;

namespace MeetingRoom.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ExamContext context)
        {
            var rooms = new Room[0];

            // Look for any rooms.
            if (!context.Rooms.Any())
            {
                rooms = new Room[]
                {
                    new Room { Name = "Room 1" },
                    new Room { Name = "Room 2" },
                    new Room { Name = "Room 3" },
                    new Room { Name = "Room 4" }
                };

                foreach (var r in rooms)
                {
                    context.Rooms.Add(r);
                }
                context.SaveChanges();
            }

            var attributes = new RoomAttribute[0];

            // Look for any room attributes.
            if (!context.RoomAttributes.Any())
            {
                attributes = new RoomAttribute[]
                {
                    new RoomAttribute { Name = "Size", Value = "400 Square Ft" },
                    new RoomAttribute { Name = "Windows", Value = "5" },
                    new RoomAttribute { Name = "Entrance Door", Value = "1" },
                    new RoomAttribute { Name = "Boardroom Style Table", Value = "10 Chairs" },
                    new RoomAttribute { Name = "Projector", Value = "" },
                    new RoomAttribute { Name = "Projector Screen", Value = "" },
                    new RoomAttribute { Name = "Flipchart", Value = "" },
                    new RoomAttribute { Name = "Conference Telephone", Value = "" }
                };

                foreach (var a in attributes)
                {
                    context.RoomAttributes.Add(a);
                }
                context.SaveChanges();
            }

            var roomItems = new RoomItem[0];

            if (!context.RoomItems.Any())
            {
                roomItems = new RoomItem[]
                {
                    new RoomItem { RoomId = rooms.Single(r => r.Name == "Room 1").Id,
                                   RoomAttributeId = attributes.Single(a => a.Name == "Size").Id
                    },
                    new RoomItem { RoomId = rooms.Single(r => r.Name == "Room 1").Id,
                                   RoomAttributeId = attributes.Single(a => a.Name == "Windows").Id
                    },
                    new RoomItem { RoomId = rooms.Single(r => r.Name == "Room 1").Id,
                                   RoomAttributeId = attributes.Single(a => a.Name == "Entrance Door").Id
                    },
                    new RoomItem { RoomId = rooms.Single(r => r.Name == "Room 3").Id,
                                   RoomAttributeId = attributes.Single(a => a.Name == "Size").Id
                    },
                    new RoomItem { RoomId = rooms.Single(r => r.Name == "Room 3").Id,
                                   RoomAttributeId = attributes.Single(a => a.Name == "Windows").Id
                    },
                    new RoomItem { RoomId = rooms.Single(r => r.Name == "Room 3").Id,
                                   RoomAttributeId = attributes.Single(a => a.Name == "Entrance Door").Id
                    },
                    new RoomItem { RoomId = rooms.Single(r => r.Name == "Room 3").Id,
                                   RoomAttributeId = attributes.Single(a => a.Name == "Projector").Id
                    },
                    new RoomItem { RoomId = rooms.Single(r => r.Name == "Room 3").Id,
                                   RoomAttributeId = attributes.Single(a => a.Name == "Projector Screen").Id
                    }
                };

                foreach (var r in roomItems)
                {
                    context.RoomItems.Add(r);
                }
                context.SaveChanges();
            }
        }
    }
}
