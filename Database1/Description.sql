CREATE TABLE [dbo].[Description]
(
	[DescriptionId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WorkerId] INT NOT NULL, 
    [Description] NVARCHAR(MAX) NOT NULL, 
    [AccountNumber] NCHAR(10) NULL, 
    [HashCode] INT NOT NULL, 
    [IsActive] BIT NOT NULL, 
    [DateLastUsed] DATETIME NULL, 
    [IsSelectable] BIT NOT NULL DEFAULT 1
)
