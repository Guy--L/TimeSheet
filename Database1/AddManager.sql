USE [TimeSheetDB]
GO
/****** Object:  Table [dbo].[Worker]    Script Date: 12/12/2013 4:58:48 PM ******/
DROP TABLE [dbo].[Worker]
GO
/****** Object:  Table [dbo].[Worker]    Script Date: 12/12/2013 4:58:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Worker](
	[WorkerId] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeNumber] [nvarchar](10) NULL,
	[LevelId] [int] NULL,
	[WorkDeptId] [int] NULL,
	[FacilityId] [int] NULL,
	[RoleId] [int] NULL,
	[FirstName] [nvarchar](30) NOT NULL,
	[LastName] [nvarchar](30) NOT NULL,
	[IsManager] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsPartTime] [bit] NOT NULL,
	[OnDisability] [bit] NOT NULL,
	[IonName] [nvarchar](50) NULL,
	[IsAdmin] [bit] NOT NULL,
	[ManagerId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[WorkerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Worker] ON 

INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 1, N'01034671', 3, 12, 7, 6, N'Brian', N'Kiley', 0, 1, 0, 0, N'Kiley.bn', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 2, N'01034951', 3, 4, 7, 6, N'Randy', N'Gross', 1, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 3, N'01045179', 3, 12, 4, 6, N'Kerry', N'Becker', 1, 1, 0, 0, N'becker.kg', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 4, N'01051237', 3, 11, 1, 6, N'Steve', N'Winbigler', 1, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 5, N'01130528', 3, 12, 4, 5, N'Kelly', N'Teegarden', 1, 1, 0, 0, N'teegarden.ks', 1)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 6, N'01138951', 3, 12, 6, 6, N'Brian', N'Burns', 1, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 7, N'01141335', 3, 12, 1, 6, N'Tim', N'Fiedeldey', 0, 1, 0, 0, N'fiedeldey.tp', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 8, N'01141807', 3, 12, 1, 6, N'April', N'Hills', 1, 1, 0, 0, N'hills.al', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 9, N'01142705', 3, 2, 7, 6, N'Peter', N'Tran', 1, 1, 0, 0, N'tran.pm', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 10, N'01146593', 3, 11, 1, 6, N'Dan', N'Williams', 1, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 11, N'01147062', 3, 12, 3, 6, N'Bryony', N'Marshall', 0, 1, 0, 0, N'marshall.be', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 12, N'01148540', 3, 12, 6, 6, N'Tom', N'Brady', 1, 1, 0, 0, N'brady.tv', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 13, N'01508001', 3, 3, 6, 6, N'Dave', N'Wise', 1, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 14, N'01605464', 3, 12, 3, 6, N'Matt', N'Paumier', 1, 1, 0, 0, N'paumier.m', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 15, N'01657999', 3, 12, 6, 6, N'Matt', N'Stophlet', 0, 1, 0, 0, N'stophlet.mg', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 16, N'01658002', 3, 12, 7, 6, N'Terrell', N'Byrd', 1, 1, 0, 0, N'byrd.td', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 17, N'01658008', 3, 12, 1, 6, N'Cai', N'Feng', 1, 1, 0, 0, N'feng.ch', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 18, N'01022062', 5, 9, 7, 3, N'Ngornly', N'Ly', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 19, N'01033524', 5, 12, 7, 3, N'Mike', N'Burke', 0, 1, 0, 0, N'burke.mc.1', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 20, N'01034926', 5, 9, 7, 3, N'Charlie', N'Mahler', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 21, N'01037965', 5, 12, 3, 3, N'Steve', N'Kolesar', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 22, N'01040146', 5, 11, 1, 3, N'Mark', N'Neumann', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 23, N'01042270', 5, 3, 6, 3, N'Greg', N'Campbell', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 24, N'01042944', 5, 12, 7, 3, N'Tom', N'Lemmink', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 25, N'01043877', 5, 11, 1, 3, N'Gary', N'Piatt', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 26, N'01044794', 5, 12, 3, 3, N'Lou', N'Zeiser', 0, 1, 0, 0, N'zeiser.lr', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 27, N'01049970', 5, 12, 6, 3, N'Gary', N'Manos', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 28, N'01053294', 5, 12, 6, 3, N'Buzz', N'Watters', 0, 1, 0, 0, N'watters.wa', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 29, N'01054834', 5, 3, 6, 3, N'Tom', N'Dierig', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 30, N'01054926', 5, 9, 7, 3, N'Dennis', N'Piper', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 31, N'01085651', 5, 12, 3, 3, N'Rusty', N'Perry', 0, 1, 0, 0, N'perry.r', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 32, N'01085674', 5, 3, 6, 3, N'Bob', N'Swensen', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 33, N'01101227', 5, 12, 1, 3, N'Doni', N'Hatz', 0, 1, 0, 0, N'hatz.dj', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 34, N'01128653', 5, 9, 7, 3, N'Tom', N'Bender', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 35, N'01130328', 5, 12, 6, 3, N'Dave', N'Anderson', 0, 1, 0, 0, N'anderson.do', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 36, N'01130454', 5, 11, 1, 3, N'Rob', N'Pfeifer', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 37, N'01130996', 5, 3, 6, 2, N'Barbara', N'Williams', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 38, N'01133520', 5, 12, 1, 3, N'John', N'Herlinger', 0, 1, 0, 0, N'herlinger.jp', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 39, N'01133589', 5, 12, 3, 3, N'Kevin', N'Tewell', 0, 1, 0, 0, N'tewell.ka', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 40, N'01142704', 5, 12, 7, 3, N'Mark', N'Wiley', 0, 1, 0, 0, N'wiley.mp', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 41, N'01142787', 5, 12, 3, 3, N'Mike', N'Schell', 0, 1, 0, 0, N'schell.mr', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 42, N'01145679', 5, 12, 7, 3, N'Mike', N'Bell', 0, 1, 0, 0, N'bell.mj', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 43, N'01145912', 5, 12, 7, 3, N'Earl', N'Osborne', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 44, N'01146264', 5, 12, 3, 3, N'Phil', N'Hughes', 0, 1, 0, 0, N'Hughes.p', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 45, N'01148760', 5, 12, 1, 3, N'Mike', N'Roaden', 0, 1, 0, 0, N'roaden.mr', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 46, N'01502060', 5, 12, 3, 3, N'Dan', N'Schmaltz', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 47, N'01502103', 5, 12, 3, 3, N'Bill', N'Dunaway', 0, 1, 0, 0, N'dunaway.ws', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 48, N'01502125', 5, 12, 3, 3, N'Bill', N'McLaughlin', 0, 1, 0, 0, N'mclaughlin.wk', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 49, N'01502862', 5, 12, 6, 3, N'Rob', N'Dawn', 0, 1, 0, 0, N'dawn.rk', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 50, N'01502863', 5, 12, 6, 3, N'Ski', N'Buchenau', 0, 1, 0, 0, N'buchenau.sk', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 51, N'01503314', 5, 12, 6, 3, N'Dean', N'Albright', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 52, N'01504668', 5, 12, 7, 3, N'Mike', N'Griffin', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 53, N'01505298', 5, 12, 3, 3, N'Freddie', N'Kendall', 0, 1, 0, 0, N'kendall.f', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 54, N'01507087', 5, 12, 6, 3, N'Doug', N'Otting', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 55, N'01512401', 5, 12, 3, 3, N'Bill', N'Meier', 0, 1, 0, 0, N'meier.we', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 56, N'01514378', 5, 12, 1, 3, N'Kirk', N'Schrotel', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 57, N'01520944', 5, 12, 1, 3, N'Rich', N'Maupin', 0, 1, 0, 1, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 58, N'01523939', 5, 9, 7, 3, N'Dan', N'Conrad', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 59, N'01525303', 5, 12, 6, 3, N'Corey', N'Moore', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 60, N'01558237', 5, 12, 3, 3, N'Rick', N'Ponton', 0, 1, 0, 0, N'ponton.rj', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 61, N'01607383', 5, 12, 3, 3, N'Michael', N'Byerly', 0, 1, 0, 0, N'byerly.ms', 1)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 62, N'01612935', 5, 12, 7, 3, N'Stanley', N'Gregory', 0, 1, 0, 0, N'Gregory.sr', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 63, N'01613624', 5, 12, 6, 3, N'Louis', N'Hudepohl', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 64, N'01629089', 5, 12, 7, 3, N'Barron', N'Weant', 0, 1, 0, 0, N'Weant.ba', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 65, N'01631373', 5, 12, 7, 3, N'Jody', N'Moye', 0, 1, 0, 0, N'Moye.j', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 66, N'01635456', 5, 12, 1, 3, N'Gregory', N'Dougherty', 0, 1, 0, 0, N'Dougherty.gr', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 67, N'01635956', 5, 12, 7, 3, N'Matt', N'Kiefer', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 68, N'01635957', 5, 12, 1, 3, N'Matt', N'Mueller', 0, 1, 0, 0, N'Mueller.mr', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 69, N'01657927', 5, 12, 7, 3, N'Mike', N'Galligan', 0, 1, 0, 0, N'Galligan.mj', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 70, N'01658001', 5, 12, 7, 3, N'Dan', N'Machenheimer', 0, 1, 0, 0, N'Machenheimer.de', 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 71, N'01658009', 5, 12, 1, 3, N'Sean', N'Thomas', 0, 1, 0, 0, NULL, 0)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 72, N'09999999', 6, 12, 1, 3, N'Mark', N'Steiner', 0, 1, 0, 0, N'steiner.ma', 1)
INSERT [dbo].[Worker] ([ManagerId], [WorkerId], [EmployeeNumber], [LevelId], [WorkDeptId], [FacilityId], [RoleId], [FirstName], [LastName], [IsManager], [IsActive], [IsPartTime], [OnDisability], [IonName], [IsAdmin]) VALUES (NULL, 73, N'08888888', 6, 12, 1, 3, N'Guy', N'Lister', 0, 1, 0, 0, N'lister.g.1', 1)
SET IDENTITY_INSERT [dbo].[Worker] OFF
