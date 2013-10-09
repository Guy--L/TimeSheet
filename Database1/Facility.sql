CREATE TABLE [dbo].[Facility]
(
	[FacilityId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Facility] NVARCHAR(30) NOT NULL, 
    [AccountsPayableID] NVARCHAR(30) NOT NULL
)
