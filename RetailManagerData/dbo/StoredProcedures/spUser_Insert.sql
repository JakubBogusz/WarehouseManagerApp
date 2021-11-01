CREATE PROCEDURE [dbo].[spUser_Insert]
	@Id nvarchar(128), 
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@EmailAddress nvarchar(256)
AS
BEGIN
	set nocount on;

	INSERT INTO dbo.[User] (Id, FirstName, LastName, EmailAddress)
	VALUES (@Id, @FirstName, @LastName, @EmailAddress);
END