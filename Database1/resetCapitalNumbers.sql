delete from WorkerCapitalNumber
insert into WorkerCapitalNumber (workerid, capitalnumber) 
select distinct workerid, capitalnumber from [week] 
where AccountType = 2 and [year] = 2014 and weeknumber > 11 