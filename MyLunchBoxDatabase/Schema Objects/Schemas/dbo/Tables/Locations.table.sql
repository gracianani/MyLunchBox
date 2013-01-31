CREATE TABLE [dbo].[Locations]
(
	LocationId int primary key identity(1,1), 
	LocationName nvarchar(200) not null,
	BusinessName varchar(200),
	FirstName varchar(200) not null,
	LastName varchar(200) not null,
	Address1 varchar(200) not null,
	Address2 varchar(200) null,
	City varchar(200) not null,
	StateOrProvince varchar(200) not null,
	Country varchar(50) not null,
	CountryCode varchar(10) not null,
	phoneNumber varchar(20) null,
	ZipCode varchar(10) not null
)
