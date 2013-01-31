/*
Deployment script for MyLunchBoxDatabase
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "MyLunchBoxDatabase"
:setvar DefaultDataPath ""
:setvar DefaultLogPath ""

GO
:on error exit
GO
USE [master]
GO
IF (DB_ID(N'$(DatabaseName)') IS NOT NULL
    AND DATABASEPROPERTYEX(N'$(DatabaseName)','Status') <> N'ONLINE')
BEGIN
    RAISERROR(N'The state of the target database, %s, is not set to ONLINE. To deploy to this database, its state must be set to ONLINE.', 16, 127,N'$(DatabaseName)') WITH NOWAIT
    RETURN
END

GO
IF (DB_ID(N'$(DatabaseName)') IS NOT NULL) 
BEGIN
    ALTER DATABASE [$(DatabaseName)]
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$(DatabaseName)];
END

GO
PRINT N'Creating $(DatabaseName)...'
GO
CREATE DATABASE [$(DatabaseName)]
    ON 
    PRIMARY(NAME = [MyLunchBoxDevelopment], FILENAME = '$(DefaultDataPath)$(DatabaseName).mdf', SIZE = 2048 KB, FILEGROWTH = 1024 KB)
    LOG ON (NAME = [MyLunchBoxDevelopment_log], FILENAME = '$(DefaultLogPath)$(DatabaseName)_log.ldf', SIZE = 1024 KB, MAXSIZE = 2097152 MB, FILEGROWTH = 10 %) COLLATE SQL_Latin1_General_CP1_CI_AS
GO
EXECUTE sp_dbcmptlevel [$(DatabaseName)], 100;


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ANSI_NULLS ON,
                ANSI_PADDING ON,
                ANSI_WARNINGS ON,
                ARITHABORT ON,
                CONCAT_NULL_YIELDS_NULL ON,
                NUMERIC_ROUNDABORT OFF,
                QUOTED_IDENTIFIER ON,
                ANSI_NULL_DEFAULT ON,
                CURSOR_DEFAULT LOCAL,
                RECOVERY FULL,
                CURSOR_CLOSE_ON_COMMIT OFF,
                AUTO_CREATE_STATISTICS ON,
                AUTO_SHRINK OFF,
                AUTO_UPDATE_STATISTICS ON,
                RECURSIVE_TRIGGERS OFF 
            WITH ROLLBACK IMMEDIATE;
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_CLOSE OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ALLOW_SNAPSHOT_ISOLATION OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET READ_COMMITTED_SNAPSHOT OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_UPDATE_STATISTICS_ASYNC OFF,
                PAGE_VERIFY NONE,
                DATE_CORRELATION_OPTIMIZATION OFF,
                DISABLE_BROKER,
                PARAMETERIZATION SIMPLE,
                SUPPLEMENTAL_LOGGING OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET TRUSTWORTHY OFF,
        DB_CHAINING OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'The database settings cannot be modified. You must be a SysAdmin to apply these settings.';
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET HONOR_BROKER_PRIORITY OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'The database settings cannot be modified. You must be a SysAdmin to apply these settings.';
    END


GO
USE [$(DatabaseName)]
GO
IF fulltextserviceproperty(N'IsFulltextInstalled') = 1
    EXECUTE sp_fulltext_database 'enable';


GO
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

GO
PRINT N'Creating [dbo].[BentoBox]...';


GO
CREATE TABLE [dbo].[BentoBox] (
    [BentoBoxId]          INT             IDENTITY (1, 1) NOT NULL,
    [BentoBoxName]        NVARCHAR (200)  NOT NULL,
    [BentoBoxDescription] NVARCHAR (1000) NOT NULL,
    [UnitPrice]           DECIMAL (8, 2)  NOT NULL,
    [RestaurantId]        INT             NULL,
    PRIMARY KEY CLUSTERED ([BentoBoxId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating [dbo].[CustomBentoBox]...';


GO
CREATE TABLE [dbo].[CustomBentoBox] (
    [CustomBentoBoxId]   INT            IDENTITY (1, 1) NOT NULL,
    [CustomBentoBoxName] NVARCHAR (200) NOT NULL,
    [BentoBoxId]         INT            NULL,
    [Comment]            NVARCHAR (200) NULL,
    PRIMARY KEY CLUSTERED ([CustomBentoBoxId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating [dbo].[CustomBentoBoxItem]...';


GO
CREATE TABLE [dbo].[CustomBentoBoxItem] (
    [CustomBentoBoxItemId] INT IDENTITY (1, 1) NOT NULL,
    [CustomBentoBoxId]     INT NULL,
    [DishId]               INT NULL,
    PRIMARY KEY CLUSTERED ([CustomBentoBoxItemId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating [dbo].[Dishes]...';


GO
CREATE TABLE [dbo].[Dishes] (
    [dishId]              INT             IDENTITY (1, 1) NOT NULL,
    [dishName]            NVARCHAR (100)  NOT NULL,
    [dishType]            NVARCHAR (100)  NOT NULL,
    [dishStatusId]        INT             NOT NULL,
    [shortDescription]    NVARCHAR (500)  NULL,
    [detailedDescription] NVARCHAR (2000) NULL,
    [restaurantId]        INT             NULL,
    [dishImageUrl]        VARCHAR (200)   NULL,
    PRIMARY KEY CLUSTERED ([dishId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating [dbo].[Locations]...';


GO
CREATE TABLE [dbo].[Locations] (
    [LocationId]      INT            IDENTITY (1, 1) NOT NULL,
    [LocationName]    NVARCHAR (200) NOT NULL,
    [BusinessName]    VARCHAR (200)  NULL,
    [FirstName]       VARCHAR (200)  NOT NULL,
    [LastName]        VARCHAR (200)  NOT NULL,
    [Address1]        VARCHAR (200)  NOT NULL,
    [Address2]        VARCHAR (200)  NOT NULL,
    [City]            VARCHAR (200)  NOT NULL,
    [StateOrProvince] VARCHAR (200)  NOT NULL,
    [Country]         VARCHAR (50)   NOT NULL,
    [CountryCode]     VARCHAR (10)   NOT NULL,
    [phoneNumber]     VARCHAR (20)   NULL,
    PRIMARY KEY CLUSTERED ([LocationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating [dbo].[Movies]...';


GO
CREATE TABLE [dbo].[Movies] (
    [movieId]     INT           IDENTITY (1, 1) NOT NULL,
    [title]       VARCHAR (100) NULL,
    [releaseDate] DATETIME      NULL,
    [genre]       VARCHAR (100) NULL,
    [price]       DECIMAL (18)  NULL,
    [rating]      VARCHAR (30)  NULL,
    PRIMARY KEY CLUSTERED ([movieId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating [dbo].[Restaurants]...';


GO
CREATE TABLE [dbo].[Restaurants] (
    [RestaurantId]         INT            IDENTITY (1, 1) NOT NULL,
    [RestaurantName]       NVARCHAR (100) NOT NULL,
    [RestaurantStatusId]   INT            NOT NULL,
    [RestaurantHoursFrom]  DATETIME       NULL,
    [RestaurantHoursTo]    DATETIME       NULL,
    [RestaurantHours2From] DATETIME       NULL,
    [RestaurantHours2To]   DATETIME       NULL,
    [RestaurantLocationId] INT            NULL,
    PRIMARY KEY CLUSTERED ([RestaurantId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating On column: RestaurantId...';


GO
ALTER TABLE [dbo].[BentoBox] WITH NOCHECK
    ADD FOREIGN KEY ([RestaurantId]) REFERENCES [dbo].[Restaurants] ([RestaurantId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating On column: BentoBoxId...';


GO
ALTER TABLE [dbo].[CustomBentoBox] WITH NOCHECK
    ADD FOREIGN KEY ([BentoBoxId]) REFERENCES [dbo].[BentoBox] ([BentoBoxId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating On column: CustomBentoBoxId...';


GO
ALTER TABLE [dbo].[CustomBentoBoxItem] WITH NOCHECK
    ADD FOREIGN KEY ([CustomBentoBoxId]) REFERENCES [dbo].[CustomBentoBox] ([CustomBentoBoxId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating On column: DishId...';


GO
ALTER TABLE [dbo].[CustomBentoBoxItem] WITH NOCHECK
    ADD FOREIGN KEY ([DishId]) REFERENCES [dbo].[Dishes] ([dishId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating On column: restaurantId...';


GO
ALTER TABLE [dbo].[Dishes] WITH NOCHECK
    ADD FOREIGN KEY ([restaurantId]) REFERENCES [dbo].[Restaurants] ([RestaurantId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating RestaurantLocation...';


GO
ALTER TABLE [dbo].[Restaurants] WITH NOCHECK
    ADD CONSTRAINT [RestaurantLocation] FOREIGN KEY ([RestaurantLocationId]) REFERENCES [dbo].[Locations] ([LocationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
-- Refactoring step to update target server with deployed transaction logs
CREATE TABLE  [dbo].[__RefactorLog] (OperationKey UNIQUEIDENTIFIER NOT NULL PRIMARY KEY)
GO
sp_addextendedproperty N'microsoft_database_tools_support', N'refactoring log', N'schema', N'dbo', N'table', N'__RefactorLog'
GO

GO
/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[Restaurants] WITH CHECK CHECK CONSTRAINT [RestaurantLocation];


GO
CREATE TABLE [#__checkStatus] (
    [Table]      NVARCHAR (270),
    [Constraint] NVARCHAR (270),
    [Where]      NVARCHAR (MAX)
);

SET NOCOUNT ON;


GO
INSERT INTO [#__checkStatus]
EXECUTE (N'DBCC CHECKCONSTRAINTS (N''[dbo].[BentoBox]'')
    WITH NO_INFOMSGS');

IF @@ROWCOUNT > 0
    BEGIN
        DROP TABLE [#__checkStatus];
        RAISERROR (N'An error occured while verifying constraints on table [dbo].[BentoBox]', 16, 127);
    END


GO
INSERT INTO [#__checkStatus]
EXECUTE (N'DBCC CHECKCONSTRAINTS (N''[dbo].[CustomBentoBox]'')
    WITH NO_INFOMSGS');

IF @@ROWCOUNT > 0
    BEGIN
        DROP TABLE [#__checkStatus];
        RAISERROR (N'An error occured while verifying constraints on table [dbo].[CustomBentoBox]', 16, 127);
    END


GO
INSERT INTO [#__checkStatus]
EXECUTE (N'DBCC CHECKCONSTRAINTS (N''[dbo].[CustomBentoBoxItem]'')
    WITH NO_INFOMSGS');

IF @@ROWCOUNT > 0
    BEGIN
        DROP TABLE [#__checkStatus];
        RAISERROR (N'An error occured while verifying constraints on table [dbo].[CustomBentoBoxItem]', 16, 127);
    END


GO
INSERT INTO [#__checkStatus]
EXECUTE (N'DBCC CHECKCONSTRAINTS (N''[dbo].[Dishes]'')
    WITH NO_INFOMSGS');

IF @@ROWCOUNT > 0
    BEGIN
        DROP TABLE [#__checkStatus];
        RAISERROR (N'An error occured while verifying constraints on table [dbo].[Dishes]', 16, 127);
    END


GO
SET NOCOUNT OFF;

DROP TABLE [#__checkStatus];


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        DECLARE @VarDecimalSupported AS BIT;
        SELECT @VarDecimalSupported = 0;
        IF ((ServerProperty(N'EngineEdition') = 3)
            AND (((@@microsoftversion / power(2, 24) = 9)
                  AND (@@microsoftversion & 0xffff >= 3024))
                 OR ((@@microsoftversion / power(2, 24) = 10)
                     AND (@@microsoftversion & 0xffff >= 1600))))
            SELECT @VarDecimalSupported = 1;
        IF (@VarDecimalSupported > 0)
            BEGIN
                EXECUTE sp_db_vardecimal_storage_format N'$(DatabaseName)', 'ON';
            END
    END


GO
ALTER DATABASE [$(DatabaseName)]
    SET MULTI_USER 
    WITH ROLLBACK IMMEDIATE;


GO
