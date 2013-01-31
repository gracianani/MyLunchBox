CREATE PROCEDURE [dbo].[DishOrderCount_Fetch]
	@from datetime,
	@to datetime
AS
select dish.dishid, count(item.dishId) as 'dishOrderTimes'
from dbo.orderitems oi
join dbo.Orders ord on 
ord.OrderId = oi.orderId
join dbo.custombentobox custombox
on (oi.itemid = custombox.custombentoboxid
and oi.itemtypeid=1)
join dbo.custombentoboxitem item
on item.custombentoboxid = custombox.custombentoboxid
right join dbo.dishes dish
on dish.dishId = item.dishId
where ord.OrderReceivedAt between @from and @to
group by item.dishid, dish.dishId