
CREATE TABLE [dbo].[Rewards]
(
	RewardId int primary key identity(1,1), 
	RewareTypeId int foreign key references [dbo].[RewardTypes] not null,
	DiscountAmount decimal(8,2) not null,
	RewardCreatedAt datetime not null,
	RewardDescription varchar(100),
	UserId int foreign key references [dbo].[users],
	IsUsed bit not null default 0
)
