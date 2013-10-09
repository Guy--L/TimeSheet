CREATE TABLE [dbo].[Worker]
(
	[WorkerId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [EmployeeNumber] NVARCHAR(10) NULL, 
    [LevelId] INT NULL, 
    [WorkDeptId] INT NULL, 
    [FacilityId] INT NULL, 
    [RoleId] INT NULL, 
    [FirstName] NVARCHAR(30) NOT NULL, 
    [LastName] NVARCHAR(30) NOT NULL, 
    [IsManager] BIT NOT NULL, 
    [IsActive] BIT NOT NULL, 
    [IsPartTime] BIT NOT NULL, 
    [OnDisability] BIT NOT NULL, 
    [IonName] NVARCHAR(50) NULL
)
