CREATE TABLE [dbo].[ShoppingCarts]
(
	ShoppingCartId int primary key identity(1,1),
	UserId int foreign key references [dbo].[Users],
	CreatedAt datetime not null,
	LastUpdatedAt datetime not null
)


