create table dbo.votings(
votingId int primary key identity(1,1),
dishId int foreign key references dbo.dishes not null,
votedOn datetime not null,
votedBy int foreign key references dbo.users not null
)