/****** Object:  Stored Procedure dbo.LogDetail_Insert    Script Date: 5/16/2016 2:20:12 PM ******/
/****** Object:  Stored Procedure dbo.LogDetail_Insert    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.LogDetail_Insert    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.LogDetail_Insert    Script Date: 4/6/2016 11:27:47 AM ******/

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
DB: NexEPA
Original: 

Revisions:  

*/
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.LogDetail (
		[ActorId]
		,[SpName]
		,[LogDate]
		,[ObjectId]
		,[ObjectTypeId]
		,[ActionId]
		,[Data]
		,[OldData]
		,[ObjectIds]
	)
	SELECT @ActorId
		,@SpName
		,GETDATE()
		,@ObjectId
		,@ObjectTypeId
		,[Id]
		,@Data
		,@OldData
		,@ObjectIds
	FROM [dbo].[LogAction] WITH (NOLOCK)
	WHERE [SpName] = @SpName
END