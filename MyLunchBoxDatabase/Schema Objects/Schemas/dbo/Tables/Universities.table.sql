CREATE TABLE [dbo].[Universities]
(
	UniversityId int primary key identity(1,1), 
	UniversityName nvarchar(200) not null,
	UniversityLocationId int foreign key references [dbo].[Locations](locationId) not null
)
