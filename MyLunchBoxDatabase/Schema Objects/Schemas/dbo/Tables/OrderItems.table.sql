
CREATE TABLE [dbo].[OrderItems]
(
	orderItemId int primary key identity(1,1),
	orderId int foreign key references [dbo].[orders] not null,
	itemId int not null,
	itemTypeId int foreign key references [dbo].[itemTypes] not null,
	quantity int not null,
	lineItemCost decimal(8,2) not null 
)