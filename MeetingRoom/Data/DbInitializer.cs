﻿using MeetingRoom.Models;
using System;
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

            var foodItems = new Food[0];

            if (!context.FoodItems.Any())
            {
                foodItems = new Food[]
                {
                    new Food { Name = "Tea" },
                    new Food { Name = "Coffee" },
                    new Food { Name = "Biscuits" },
                    new Food { Name = "Buffets"}
                };

                foreach (var f in foodItems)
                {
                    context.FoodItems.Add(f);
                }
                context.SaveChanges();
            }

            var schedules = new Schedule[0];

            if (!context.Schedules.Any())
            {
                DateTime now = DateTime.Now;

                schedules = new Schedule[]
                {
                    new Schedule { RoomId = rooms.Single(r => r.Name == "Room 1").Id,
                                   Name = "Schedule 1",
                                   StartTime = new DateTime(now.Year, now.Month, now.Day) + new TimeSpan(7, 0, 0),
                                   EndTime = new DateTime(now.Year, now.Month, now.Day) + new TimeSpan(10, 0, 0)
                    },
                    new Schedule { RoomId = rooms.Single(r => r.Name == "Room 1").Id,
                                   Name = "Schedule 2",
                                   StartTime = new DateTime(now.Year, now.Month, now.Day) + new TimeSpan(13, 0, 0),
                                   EndTime = new DateTime(now.Year, now.Month, now.Day) + new TimeSpan(15, 0, 0)
                    },
                };

                foreach (var s in schedules)
                {
                    context.Schedules.Add(s);
                }
                context.SaveChanges();
            }
        }
    }
}
