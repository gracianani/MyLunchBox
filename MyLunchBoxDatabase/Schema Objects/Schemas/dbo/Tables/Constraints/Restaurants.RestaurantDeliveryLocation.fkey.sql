ALTER TABLE [dbo].[Restaurants]
ADD RestaurantDeliveryLocationId int foreign key REFERENCES [dbo].[locations](locationId)	

