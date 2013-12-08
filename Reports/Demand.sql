select
h.AccountNumber,
w.EmployeeNumber,
sum(h.WeekHours*(case h.IsOvertime when 0 then v.RegularRate when 1 then v.OvertimeRate end)) Amount
from dbo.HoursByWeek h
inner join worker w on h.WorkerId = w.WorkerId
inner join [level] v on w.LevelId = v.LevelId
group by h.AccountNumber, w.EmployeeNumber

select * from dbo.week