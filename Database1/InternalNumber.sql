CREATE TABLE [dbo].[InternalNumber]
(
	[InternalNumberId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [InternalOrder] NVARCHAR(50) NOT NULL, 
    [LegalEntity] NVARCHAR(50) NOT NULL
)
