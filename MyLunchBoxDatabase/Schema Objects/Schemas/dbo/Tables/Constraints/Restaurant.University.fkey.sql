ALTER TABLE [Restaurants]
ADD universityId int FOREIGN KEY REFERENCES [dbo].[Universities](universityId) default 1
