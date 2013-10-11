delete from worker
insert into Worker (EmployeeNumber, LevelId, RoleId, WorkDeptId, FacilityId, FirstName, LastName, IsManager, IsPartTime, IsActive, OnDisability, IonName)
select e.employeenumber, v.LevelId, r.RoleId, w.WorkDeptId, fc.FacilityId, FirstName, LastName, IsManager, IsPartTime, IsInactive^1, OnDisability, IonName from [GUYLISTERC90F\DEV].timesheet.dbo.tblEmployee e
	join [Level] v on v.[Level] = e.CurrentLevel
	join [GUYLISTERC90F\DEV].timesheet.dbo.tlkpLaborType b on b.LaborTypeID = e.LaborTypeID
	join [Role] r on r.[Role] = b.LaborType
	join [WorkDept] w on w.WorkDept = e.WorkDept
	join [GUYLISTERC90F\DEV].timesheet.dbo.tlkpFacility f on f.FacilityID = e.FacilityID
	join [Facility] fc on fc.Facility = f.Facility

select e.* from [GUYLISTERC90F\DEV].timesheet.dbo.tblEmployee e
	left join [Worker] w on w.EmployeeNumber = e.EmployeeNumber
	where w.EmployeeNumber is null

select e.* from [GUYLISTERC90F\DEV].timesheet.dbo.tblEmployee e
	join [GUYLISTERC90F\DEV].timesheet.dbo.tlkpFacility f on f.FacilityID = e.FacilityID
	join [Facility] fc on fc.FacilityId = f.FacilityID

select * from [GUYLISTERC90F\DEV].timesheet.dbo.tlkpFacility f 
	left join [Facility] fc on fc.Facility = f.Facility
	where fc.FacilityId is null
