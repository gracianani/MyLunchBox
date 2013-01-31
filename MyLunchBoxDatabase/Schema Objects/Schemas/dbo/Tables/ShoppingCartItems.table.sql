CREATE TABLE [dbo].[ShoppingCartItems]
(
	ShoppingCartItemId int primary key identity(1,1),
	ShoppingCartId int foreign key references [dbo].[ShoppingCarts] NOT NULL, 
	Quantity int not null,
	ItemId int not null,
	ItemTypeId int foreign key references [dbo].[ItemTypes] not null,
	CreatedAt datetime not null,
	LastUpdatedAt datetime not null
)
