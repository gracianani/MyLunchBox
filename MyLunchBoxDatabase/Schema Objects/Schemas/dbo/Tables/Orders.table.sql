CREATE TABLE [dbo].[Orders]
(
	OrderId int primary key identity(1,1),
	TxnId varchar(50) null,
	UserId int foreign key references [dbo].[Users],
	PayerEmail varchar(100) not null,
	OrderStatusId int foreign key references [dbo].[OrderStatusLevels],
	OrderReceivedAt datetime not null,
	OrderDescription nvarchar(500) not null,
	PaymentStatus varchar(50) not null,
	Gross decimal(8,2) not null,
	Fee decimal(8,2) not null,
	Tax decimal(8,2) not null,
	PaymentNote nvarchar(200) not null,
	Savings decimal(8,2) not null default 0,
	ReceiverFirstName nvarchar(50),
	ReceiverLastName nvarchar(50),
	DeliveryLocationId int foreign key references [dbo].[locations] not null default 1,
	DeliveryTime datetime not null default getdate()
)
