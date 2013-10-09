CREATE TABLE [dbo].[WorkDept]
(
	[WorkDeptId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WorkDept] NVARCHAR(10) NOT NULL, 
    [WorkDeptDesc] NVARCHAR(50) NOT NULL, 
    [ProcessId] INT NULL
)
