CREATE TABLE [dbo].[Dishes]
(
	dishId int primary key identity(1,1) NOT NULL, 
	dishName nvarchar(100) not null,
	dishTypeId int foreign key references [dbo].[dishTypes],
	dishStatusId int not null,
	shortDescription nvarchar(500),
	detailedDescription nvarchar(2000),
	restaurantId int foreign key references[dbo].[restaurants],
	dishImageUrl varchar(200),
	dishIncrementalPrice decimal(10,2) default(0.0) not null,
	isOnVoting bit not null default 0

)

