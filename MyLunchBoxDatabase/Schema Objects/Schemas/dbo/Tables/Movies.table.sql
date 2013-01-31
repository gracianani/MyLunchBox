CREATE TABLE [dbo].[Movies] (
    [movieId]     INT           IDENTITY (1, 1) NOT NULL,
    [title]       VARCHAR (100) NULL,
    [releaseDate] DATETIME      NULL,
    [genre]       VARCHAR (100) NULL,
    [price]       DECIMAL (18)  NULL,
    [rating]      VARCHAR (30)  NULL,
    PRIMARY KEY CLUSTERED ([movieId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);

