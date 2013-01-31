CREATE TABLE [dbo].[CustomBentoBox]
(
	CustomBentoBoxId int primary key identity(1,1),
	CustomBentoBoxName nvarchar(200) not null,
	BentoBoxId int foreign key references [dbo].[BentoBox](BentoBoxId),
	Comment nvarchar(200) null
)
