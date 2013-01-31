CREATE TABLE [dbo].[BentoBox]
(
	BentoBoxId int primary key identity(1,1), 
	BentoBoxName nvarchar(200) not null,
	BentoBoxDescription nvarchar(1000) not null,
	UnitPrice decimal(8,2) not null,
	RestaurantId int foreign key references[dbo].[Restaurants](RestaurantId), 
	BentoBoxTypeId int foreign key references [dbo].[BentoBoxTypes],
	bentoBoxStatusId int foreign key references [dbo].[bentoBoxStatusLevels] not null
)
