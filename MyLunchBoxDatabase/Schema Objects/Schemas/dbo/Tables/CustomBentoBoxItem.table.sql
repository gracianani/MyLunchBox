CREATE TABLE [dbo].[CustomBentoBoxItem]
(
	CustomBentoBoxItemId int primary key identity(1,1),
	CustomBentoBoxId int foreign key references [dbo].[CustomBentoBox](customBentoBoxId),
	DishId int foreign key references [dbo].[Dishes](dishId)
)
