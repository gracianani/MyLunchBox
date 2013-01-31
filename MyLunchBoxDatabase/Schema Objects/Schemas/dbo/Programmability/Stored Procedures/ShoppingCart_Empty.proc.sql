CREATE PROCEDURE [dbo].[ShoppingCart#Empty]
	@ShoppingCartId int
AS
	Delete [dbo].[ShoppingCartItems] where shoppingCartId = @shoppingCartId
    Delete [dbo].[ShoppingCarts] where shoppingCartId = @shoppingCartId
	
