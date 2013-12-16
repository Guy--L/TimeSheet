CREATE TABLE [dbo].[WorkerInternalNumber]
(
	[WorkerInternalNumberId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WorkerId] INT NOT NULL, 
    [InternalNumberId] INT NOT NULL
)
