
CREATE TABLE [dbo].[UserDetails]
(
	userDetailsId int primary key identity(1,1),
	userId int foreign key references [dbo].[Users] not null,
	firstName nvarchar(50),
	lastName nvarchar(50),
	universityId int foreign key references [dbo].[universities] ,
	phoneNumber varchar(20) ,
	locationId int foreign key references [dbo].locations
)
