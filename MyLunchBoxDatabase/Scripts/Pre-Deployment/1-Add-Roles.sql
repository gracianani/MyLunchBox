CREATE TABLE [dbo].[Roles]
(
	roleId int primary key identity(1,1),
	roleName varchar(20) not null
)

CREATE TABLE [dbo].[UsersInRoles]
(
	usersInRolesId int primary key identity (1,1) ,
	userId int foreign key references [dbo].[users] not null,
	roleId int foreign key references [dbo].[roles] not null
)
