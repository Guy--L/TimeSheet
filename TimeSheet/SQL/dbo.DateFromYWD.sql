-- =============================================
-- Author:		Guy..Lister
-- Create date: 12/14/2013
-- Description:	Get date from year, week, dow
-- =============================================
CREATE FUNCTION [dbo].[DateFromYWD] 
(
@Year INT,
@Week INT,
@Weekday INT
)
RETURNS DATETIME
AS
BEGIN
RETURN CASE
WHEN @Year < 1900 OR @Year > 9999 THEN NULL
WHEN @Week < 1 OR @Week > 53 THEN NULL
WHEN @Weekday < 1 OR @Weekday > 7 THEN NULL
WHEN @Year = 9999 AND @Week = 52 And @Weekday > 5 THEN NULL
WHEN DATEPART(YEAR, DATEADD(DAY, 7 * @Week + DATEDIFF(DAY, 4, DATEADD(YEAR, @Year - 1900, 7)) / 7 * 7, -4)) <> @Year THEN NULL
ELSE DATEADD(DAY, 7 * @Week + DATEDIFF(DAY, 4, DATEADD(YEAR, @Year - 1900, 7)) / 7 * 7, @Weekday - 8)
END
END