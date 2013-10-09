CREATE TABLE [dbo].[Week]
(
	[WeekId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WeekNumber] INT NOT NULL, 
    [Year] INT NOT NULL, 
    [EmployeeId] INT NOT NULL, 
    [DescriptionId] INT NOT NULL, 
    [Comments] NVARCHAR(MAX) NULL, 
    [IsOvertime] BIT NOT NULL, 
    [Monday] MONEY NULL, 
    [Tuesday] MONEY NULL, 
    [Wednesday] MONEY NULL, 
    [Thursday] MONEY NULL, 
    [Friday] MONEY NULL, 
    [Saturday] MONEY NULL, 
    [Sunday] MONEY NULL, 
    [Submitted] DATETIME NULL
)
