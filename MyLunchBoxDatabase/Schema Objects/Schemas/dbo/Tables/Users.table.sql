

CREATE TABLE [dbo].[Users]
(
	userId int primary key identity(1,1) ,
	email varchar(100) not null,
	hashedPAssword nvarchar(128) not null,
	salt nvarchar(128) not null,
	strkey nvarchar(128) not null,
	firstName nvarchar(50) not null,
	lastName nvarchar(50) not null,
	phoneNumber varchar(15) null,
	createdOn datetime not null,
	referrer varchar(100) null,
	lastActivityAt datetime not null,
	lastLoginAt datetime not null,
	lastLockoutAt datetime not null,
	lastPasswordChangedAt datetime not null,
	isLockedOut bit not null,
	isApproved bit not null,
	facebookUserId bigint null,
	receiveEmail bit not null,
	isConfirmed bit not null,
	addressId int foreign key references [dbo].[Locations],
	fbAccessToken varchar(255) null,
	universityId int foreign key references [dbo].[Universities],
	passwordQuestion nvarchar(255) not null,
	passwordAnswer nvarchar(255) not null,
	failedPasswordAttemptCount int not null,
	failedPasswordAnswerAttemptCount int not null
)
