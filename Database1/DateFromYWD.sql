-- =============================================
-- Author:		Guy..Lister
-- Create date: 12/14/2013
-- Description:	Get date from year, week, dow
-- =============================================
CREATE FUNCTION [dbo].[DateFromYWD] 
(
	-- Add the parameters for the function here
	@Year int,
	@Week int,
	@DOW int
)
RETURNS DateTime
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result DateTime,
		@StartDate DateTime,
	    @FirstDayOfYear DATETIME,
	    @FirstMondayOfYear DATETIME

	-- Get the first day of the provided year.
	SET @FirstDayOfYear = CAST('1/1/' + CAST(@Year AS VARCHAR) AS DATETIME)

	-- Get the first monday of the year, then add the number of weeks.
	SET @FirstMondayOfYear = DATEADD(WEEK, DATEDIFF(WEEK, 0, DATEADD(DAY, 6 - DATEPART(DAY, @FirstDayOfYear), @FirstDayOfYear)), 0)

	SET @StartDate = DATEADD(week, @Week - 2, @FirstMondayOfYear)			-- I don't trust these two lines
	set @Result = DateAdd(day, @DOW-2, @StartDate)

	-- Return the result of the function
	RETURN @Result

END