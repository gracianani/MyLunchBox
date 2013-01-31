CREATE TABLE [dbo].[University_Delivery]
(
	universityDeliveryId int primary key identity(1,1),
	universityId  int foreign key references [dbo].[universities] not null,
	locationId int foreign key references [dbo].[locations] not null,
	deliveryTime datetime not null
)
