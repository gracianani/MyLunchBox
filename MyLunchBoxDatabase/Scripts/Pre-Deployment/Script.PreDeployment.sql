/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

SET IDENTITY_INSERT [dbo].[itemTypes] ON
	INSERT INTO [dbo].[ItemTypes] (ItemTypeId, ItemTypeDescription)
	VALUES (1, 'CustomBentoBox')
	INSERT INTO [dbo].[ItemTypes] (ItemTypeId, ItemTypeDescription)
	VALUES (2, 'Beverage')
	INSERT INTO [dbo].[ItemTypes] (ItemTypeId, ItemTypeDescription)
	VALUES (3, 'MembershipCard')
SET IDENTITY_INSERT [dbo].[itemTypes] OFF

INSERT INTO [dbo].[OrderStatusLevels] (orderStatusId, OrderStatusDescription)
VALUES(1, 'Pending')

INSERT INTO [dbo].[orderStatusLevels] (orderStatusId, OrderStatusDescription)
VALUES(2, 'Processing')

INSERT INTO [dbo].[orderStatusLevels] (orderStatusId, OrderStatusDescription)
VALUES(3, 'Holded')

INSERT INTO [dbo].[orderStatusLevels] (orderStatusId, OrderStatusDescription)
VALUES(4, 'Complete')

INSERT INTO [dbo].[orderStatusLevels] (orderStatusId, OrderStatusDescription)
VALUES(5, 'Closed')

INSERT INTO [dbo].[orderStatusLevels] (orderStatusId, OrderStatusDescription)
VALUES(6, 'Canceled')

INSERT INTO [dbo].[DishTypes] ( DishTypeDescription)
VALUES('Main Cource')

INSERT INTO [dbo].[DishTypes] ( DishTypeDescription)
VALUES('Side Dish')

INSERT INTO [dbo].[DishTypes] ( DishTypeDescription)
VALUES('Beverage')

set identity_insert [dbo].[BentoBoxTypes] on
	insert into [dbo].[BentoBoxTypes](bentoBoxTypeId, bentoBoxDescription)
	values(1, '1 Entry Plate' )
	
	insert into [dbo].[BentoBoxTypes](bentoBoxTypeId, bentoBoxDescription)
	values(2, '2 Entry Plate' )
	
	insert into [dbo].[BentoBoxTypes](bentoBoxTypeId, bentoBoxDescription)
	values(3, '3 Entry Plate' )
	
	insert into [dbo].[BentoBoxTypes](bentoBoxTypeId, bentoBoxDescription)
	values(4, '4 Entry Plate' )
	
	insert into [dbo].[BentoBoxTypes](bentoBoxTypeId, bentoBoxDescription)
	values(5, 'A La Carte' )
	
set identity_insert [dbo].[BentoBoxTypes] off

set identity_insert [dbo].[rewardTypes] on
	insert Into dbo.RewardTypes(rewardTypeId, rewardTypeDescription)
	values( 1, 'lucky spin')
	insert Into dbo.RewardTypes(rewardTypeId, rewardTypeDescription)
	values( 2, 'membership reward')
set identity_insert [dbo].[rewardTypes] off

insert into [dbo].[bentoBoxStatusLevels] (bentoBoxStatusDescription)
values('available')
insert into [dbo].[bentoBoxStatusLevels] (bentoBoxStatusDescription)
values('unavailable')