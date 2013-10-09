CREATE TABLE [dbo].[Level]
(
	[LevelId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Level] NCHAR(5) NOT NULL, 
    [RegularRate] MONEY NOT NULL, 
    [OvertimeRate] MONEY NOT NULL
)
