CREATE TABLE [dbo].[CostCenter]
(
	[CostCenterId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CostCenter] VARCHAR(50) NOT NULL, 
    [LegalEntity] VARCHAR(50) NOT NULL
)
