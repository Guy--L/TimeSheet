CREATE LOGIN [NA\lister.g.1] FROM WINDOWS;
GO;

EXEC sp_addsrvrolemember N'NA\lister.g.1', N'sysadmin';
GO;