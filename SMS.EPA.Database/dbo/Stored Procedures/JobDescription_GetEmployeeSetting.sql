
CREATE PROCEDURE [dbo].[JobDescription_GetEmployeeSetting] @GroupId INT = NULL
	,@EmployeeId INT = NULL
	,@ShowAll BIT = 0
AS
/*
Created: 20210426@Tim.Trinh
Task: job description - get employee setting
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT emp.EmployeeId
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,emp.[Enabled]
		,gr.[Name] AS GroupName
		,gr.Code AS GroupCode
		,job.[Name] AS JobLevel
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.JobLevel AS job WITH (NOLOCK) ON emp.JobLevelId = job.JobLevelId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND (
			@GroupId IS NULL
			OR gr.GroupId = @GroupId
			)
		AND (
			@EmployeeId IS NULL
			OR emp.EmployeeId = @EmployeeId
			)
		AND (
			(
				@ShowAll = 0
				AND EXISTS (
					SELECT 1
					FROM dbo.JobDescription_EmployeeSetting AS setting WITH (NOLOCK)
					WHERE emp.EmployeeId = setting.EmployeeId
					)
				)
			OR @ShowAll = 1
			)
	ORDER BY emp.Code, emp.NickName
END