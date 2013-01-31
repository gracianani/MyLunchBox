CREATE TABLE [dbo].[RewardCards]
(
	RewardCardId int primary key identity(1,1), 
	RewardPoints decimal(8,2) not null,
	RewardDescription nvarchar(100) not null,
	UnitPrice decimal(8,2) not null,
	IsAvailable bit not null
)