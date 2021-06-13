CREATE PROCEDURE [dbo].[spInventory_GetAll]
	@param1 int = 0,
	@param2 int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Id], [ProductId], [Quantity], [PurchasePrice], [PurchaseDate]
	FROM dbo.Inventory
END
