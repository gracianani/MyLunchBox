ALTER TABLE [dbo].[Restaurants]
	ADD CONSTRAINT [RestaurantLocation] 
	FOREIGN KEY (RestaurantLocationId)
	REFERENCES [dbo].[Locations] (locationId)	

