delete from [dbo].worker
insert into [dbo].Worker (EmployeeNumber, LevelId, RoleId, WorkDeptId, FacilityId, FirstName, LastName, IsManager, IsPartTime, IsActive, OnDisability, IonName)
select e.employeenumber, v.LevelId, r.RoleId, w.WorkDeptId, fc.FacilityId, FirstName, LastName, IsManager, IsPartTime, IsInactive^1, OnDisability, IonName 
	from [2007 TSSQL].dbo.tblEmployee e
	join [Level] v on v.[Level] = e.CurrentLevel
	join [2007 TSSQL].dbo.tlkpLaborType b on b.LaborTypeID = e.LaborTypeID
	join [Role] r on r.[Role] = b.LaborType
	join [WorkDept] w on w.WorkDept = e.WorkDept
	join [2007 TSSQL].dbo.tlkpFacility f on f.FacilityID = e.FacilityID
	join [Facility] fc on fc.Facility = f.Facility
