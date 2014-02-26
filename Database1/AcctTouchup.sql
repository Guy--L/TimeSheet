select substring(internalorder,1,4), count(substring(internalorder,1,4)) from internalnumber group by substring(internalorder,1,4) order by substring(internalorder,1,4) desc
select substring(costcenter,1,4), count(substring(CostCenter,1,4)) from costcenter group by substring(costcenter,1,4) order by substring(costcenter,1,4) desc
--select cc, count(cc) from (select substring(costcenter,1,6) cc from costcenter where substring(costcenter,1,4) = '7000') group by cc order by cc desc

update [week] set accounttype = 1 where weekid in (
select weekid from week w 
	join internalnumber i on w.internalnumberid = i.InternalNumberId
	where substring(i.internalorder,1,4) = '2000' and AccountType = 0
	)
delete from [InternalNumber] where substring(internalorder,1,4) = '2000'
delete from [WorkerInternalNumber] where workerinternalnumberid in (13, 83)
insert WorkerCostCenter (workerid, costcenterid) values (44, 296)

select weekid, w.workerid, internalorder, workerinternalnumberid from week w 
	join internalnumber i on w.internalnumberid = i.InternalNumberId
	join workerinternalnumber c on c.InternalNumberId = i.InternalNumberId
	where substring(i.internalorder,1,4) = '2000' and AccountType = 0

select w.WorkerCostCenterId, c.costcenter from WorkerCostCenter w 
	join costcenter c on c.CostCenterId = w.Costcenterid
	where w.workerid=44 and substring(c.costcenter, 1,4)='2000'

select * from costcenter where costcenter = '2000107415'