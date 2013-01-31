CREATE TABLE [dbo].[DishTypes]
(
	DishTypeId int primary key identity(1,1), 
	DishTypeDescription nvarchar(100) not null
)
