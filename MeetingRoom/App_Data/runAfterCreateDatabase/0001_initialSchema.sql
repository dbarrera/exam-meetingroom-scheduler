USE [Exam-MeetingRoom]
GO
/****** Object:  Table [dbo].[Food]    Script Date: 5/2/2019 16:35:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Food](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Food] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Room]    Script Date: 5/2/2019 16:35:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Room](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Room] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RoomAttribute]    Script Date: 5/2/2019 16:35:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoomAttribute](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_RoomAttribute] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RoomItem]    Script Date: 5/2/2019 16:35:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoomItem](
	[RoomID] [int] NOT NULL,
	[RoomAttributeID] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Schedule]    Script Date: 5/2/2019 16:35:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schedule](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[RoomID] [int] NOT NULL,
	[StartTime] [datetime2](7) NOT NULL,
	[EndTime] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ServingSchedule]    Script Date: 5/2/2019 16:35:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServingSchedule](
	[FoodID] [int] NOT NULL,
	[ScheduleID] [int] NOT NULL,
	[TimeIn] [datetime2](7) NOT NULL
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[RoomItem]  WITH CHECK ADD  CONSTRAINT [FK_RoomItem_Room_RoomID] FOREIGN KEY([RoomID])
REFERENCES [dbo].[Room] ([ID])
GO
ALTER TABLE [dbo].[RoomItem] CHECK CONSTRAINT [FK_RoomItem_Room_RoomID]
GO
ALTER TABLE [dbo].[RoomItem]  WITH CHECK ADD  CONSTRAINT [FK_RoomItem_RoomAttribute_RoomAttributeID] FOREIGN KEY([RoomAttributeID])
REFERENCES [dbo].[RoomAttribute] ([ID])
GO
ALTER TABLE [dbo].[RoomItem] CHECK CONSTRAINT [FK_RoomItem_RoomAttribute_RoomAttributeID]
GO
ALTER TABLE [dbo].[Schedule]  WITH CHECK ADD  CONSTRAINT [FK_Schedule_Room_RoomID] FOREIGN KEY([RoomID])
REFERENCES [dbo].[Room] ([ID])
GO
ALTER TABLE [dbo].[Schedule] CHECK CONSTRAINT [FK_Schedule_Room_RoomID]
GO
ALTER TABLE [dbo].[ServingSchedule]  WITH CHECK ADD  CONSTRAINT [FK_ServingSchedule_Food_FoodID] FOREIGN KEY([FoodID])
REFERENCES [dbo].[Food] ([ID])
GO
ALTER TABLE [dbo].[ServingSchedule] CHECK CONSTRAINT [FK_ServingSchedule_Food_FoodID]
GO
ALTER TABLE [dbo].[ServingSchedule]  WITH CHECK ADD  CONSTRAINT [FK_ServingSchedule_Schedule_ScheduleID] FOREIGN KEY([ScheduleID])
REFERENCES [dbo].[Schedule] ([ID])
GO
ALTER TABLE [dbo].[ServingSchedule] CHECK CONSTRAINT [FK_ServingSchedule_Schedule_ScheduleID]
GO
