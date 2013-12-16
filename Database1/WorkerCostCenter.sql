CREATE TABLE [dbo].[WorkerCostCenter]
(
	[WorkerCostCenterId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WorkerId] INT NOT NULL, 
    [CostCenterId] INT NOT NULL
)
