
CREATE PROCEDURE [dbo].[LogDetail_Insert] @ActorId INT
	,@SpName VARCHAR(50) = NULL
	,@ObjectId INT = NULL
	,@ObjectTypeId INT = NULL
	,@Data NVARCHAR(MAX) = NULL
	,@OldData NVARCHAR(MAX) = NULL
	,@ObjectIds NVARCHAR(MAX)
AS
/*
Created: 20151224@Josh.Phan
Task: Insert Log Detail.
DB: SMSEPA
Original: 

Revisions:  

*/
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[LogDetail] (
		[ActorId]
		,[SpName]
		,[LogDate]
		,[ObjectId]
		,[ObjectTypeId]
		--,[ActionId]
		,[Data]
		,[OldData]
		,[ObjectIds]
		)
	SELECT @ActorId
		,@SpName
		,GETDATE()
		,@ObjectId
		,@ObjectTypeId
		--,[Id]
		,@Data
		,@OldData
		,@ObjectIds
END