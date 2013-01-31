CREATE TABLE [dbo].[Announcement]
(
	AnnouncementId int primary key identity NOT NULL, 
	AnnouncementText nvarchar(1000) not null,
	CreatedAt datetime not null,
	IsShownOnHomePage bit not null
)
