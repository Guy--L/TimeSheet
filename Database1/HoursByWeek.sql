CREATE VIEW [dbo].[HoursByWeek]
	AS SELECT     h.WeekId, CASE h.accounttype WHEN 0 THEN i.InternalOrder WHEN 1 THEN c.CostCenter WHEN 2 THEN h.CapitalNumber ELSE '' END AS AccountNumber, 
                      h.Monday + h.Tuesday + h.Wednesday + h.Thursday + h.Friday + h.Saturday + h.Sunday AS WeekHours, h.WorkerId, h.SiteId, h.PartnerId, h.WorkAreaId, 
                      h.AccountType, h.IsOvertime, CASE h.IsOvertime WHEN 0 THEN (h.Monday + h.Tuesday + h.Wednesday + h.Thursday + h.Friday + h.Saturday + h.Sunday) 
                      * v.RegularRate WHEN 1 THEN (h.Monday + h.Tuesday + h.Wednesday + h.Thursday + h.Friday + h.Saturday + h.Sunday) * v.OvertimeRate END AS WeekAmount, 
                      w.LastName + ', ' + w.FirstName AS FullName, w.EmployeeNumber, h.NewRequest, h.WeekNumber, h.Year, h.Year * 100 + h.WeekNumber AS YearWeek
FROM         dbo.Week AS h INNER JOIN
                      dbo.InternalNumber AS i ON i.InternalNumberId = h.InternalNumberId INNER JOIN
                      dbo.CostCenter AS c ON c.CostCenterId = h.CostCenterId INNER JOIN
                      dbo.Worker AS w ON w.WorkerId = h.WorkerId INNER JOIN
                      dbo.[Level] AS v ON v.LevelId = w.LevelId