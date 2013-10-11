CREATE TABLE [dbo].[Level]
(
	[LevelId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Level] NVARCHAR(5) NOT NULL, 
    [RegularRate] MONEY NOT NULL, 
    [OvertimeRate] MONEY NOT NULL
)
