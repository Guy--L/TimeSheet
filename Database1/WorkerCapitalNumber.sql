CREATE TABLE [dbo].[WorkerCapitalNumber]
(
	[WorkerCapitalNumberId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WorkerId] INT NOT NULL, 
    [CapitalNumber] NVARCHAR(50) NOT NULL
)
