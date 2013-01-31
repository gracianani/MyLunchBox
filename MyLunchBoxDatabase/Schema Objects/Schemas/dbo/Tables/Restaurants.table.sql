
CREATE TABLE [dbo].[Restaurants]
(
	RestaurantId int primary key identity(1,1) NOT NULL, 
	RestaurantName nvarchar(100) NOT NULL,
	RestaurantStatusId int NOT NULL,
	RestaurantHoursFrom datetime,
	RestaurantHoursTo datetime,
	RestaurantHours2From datetime,
	RestaurantHours2To datetime,
	RestaurantLocationId int foreign key references [dbo].[Locations] (locationId) not null,
	UniversityId int foreign key references [dbo].[universities] (unversityId) not null,
	RestaurantDeliveryLocationId int foreign key references [dbo].[Locations] (locationId),
	RestaurantDeliveryTime datetime not null default '12:00',
	RestaurantDeliveryTime2 datetime, 
	RestaurantShortDescription nvarchar(100),
	RestaurantDetailedDescription nvarchar(500),
)