CREATE VIEW [dbo].[HoursByDate]
AS
SELECT  h.WeekId
		, h.WorkDate
		, h.IsOvertime
		, h.Hours
		, h.WorkerId
		, h.SiteId
		, h.PartnerId
		, h.WorkAreaId
		, CASE h.accounttype 
			WHEN 0 THEN i.InternalOrder 
			WHEN 1 THEN c.CostCenter 
			WHEN 2 THEN h.CapitalNumber
			 ELSE '' 
			END AS AccountNumber
		, CASE h.isovertime 
			WHEN 0 THEN h.hours * v.RegularRate 
			WHEN 1 THEN h.hours * v.OvertimeRate 
			END AS amount
		, w.LastName + ', ' + w.FirstName AS FullName
		, w.EmployeeNumber
		, h.NewRequest
		, h.WeekNumber
		, h.[Year]
FROM (
	SELECT WeekId
		, dbo.DateFromYWD([Year], WeekNumber, 2) AS WorkDate
		, IsOvertime
		, Monday AS Hours
		, WorkerId
		, SiteId
		, PartnerId
		, WorkAreaId
		, WorkerId 
		, AccountType
		, CapitalNumber
		, InternalNumberId
		, CostCenterId
		, NewRequest
		, WeekNumber
		, [Year]
	FROM dbo.[Week]
    UNION ALL
	SELECT WeekId
		, dbo.DateFromYWD([Year], WeekNumber, 3) AS WorkDate
		, IsOvertime
		, Tuesday AS Hours
		, WorkerId
		, SiteId
		, PartnerId
		, WorkAreaId
		, WorkerId 
		, AccountType
		, CapitalNumber
		, InternalNumberId
		, CostCenterId
		, NewRequest
		, WeekNumber
		, [Year]
	FROM dbo.[Week]
    UNION ALL
		SELECT WeekId
		, dbo.DateFromYWD([Year], WeekNumber, 4) AS WorkDate
		, IsOvertime
		, Wednesday AS Hours
		, WorkerId
		, SiteId
		, PartnerId
		, WorkAreaId
		, WorkerId 
		, AccountType
		, CapitalNumber
		, InternalNumberId
		, CostCenterId
		, NewRequest
		, WeekNumber
		, [Year]
	FROM dbo.[Week]
	UNION ALL
	SELECT WeekId
		, dbo.DateFromYWD([Year], WeekNumber, 5) AS WorkDate
		, IsOvertime
		, Thursday AS Hours
		, WorkerId
		, SiteId
		, PartnerId
		, WorkAreaId
		, WorkerId 
		, AccountType
		, CapitalNumber
		, InternalNumberId
		, CostCenterId
		, NewRequest
		, WeekNumber
		, [Year]
	FROM dbo.[Week]
    UNION ALL
	SELECT WeekId
		, dbo.DateFromYWD([Year], WeekNumber, 6) AS WorkDate
		, IsOvertime
		, Friday AS Hours
		, WorkerId
		, SiteId
		, PartnerId
		, WorkAreaId
		, WorkerId 
		, AccountType
		, CapitalNumber
		, InternalNumberId
		, CostCenterId
		, NewRequest
		, WeekNumber
		, [Year]
	FROM dbo.[Week]
	UNION ALL
	SELECT WeekId
		, dbo.DateFromYWD([Year], WeekNumber, 7) AS WorkDate
		, IsOvertime
		, Saturday AS Hours
		, WorkerId
		, SiteId
		, PartnerId
		, WorkAreaId
		, WorkerId 
		, AccountType
		, CapitalNumber
		, InternalNumberId
		, CostCenterId
		, NewRequest
		, WeekNumber
		, [Year]
	FROM dbo.[Week]
    UNION ALL
	SELECT WeekId
		, dbo.DateFromYWD([Year], WeekNumber, 1) AS WorkDate
		, IsOvertime
		, Sunday AS Hours
		, WorkerId
		, SiteId
		, PartnerId
		, WorkAreaId
		, WorkerId 
		, AccountType
		, CapitalNumber
		, InternalNumberId
		, CostCenterId
		, NewRequest
		, WeekNumber
		, [Year]
	FROM dbo.[Week]
) AS h 
	LEFT OUTER JOIN dbo.InternalNumber AS i ON i.InternalNumberId = h.InternalNumberId 
	LEFT OUTER JOIN dbo.CostCenter AS c ON c.CostCenterId = h.CostCenterId 
	INNER JOIN dbo.Worker AS w ON w.WorkerId = h.WorkerId 
	INNER JOIN dbo.[Level] AS v ON v.LevelId = w.LevelId


