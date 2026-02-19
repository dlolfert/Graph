
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetMonday] 
(
	-- Add the parameters for the function here
	@StartDate DateTime
)
RETURNS varchar(10)
AS
BEGIN

IF DATEPART(WEEKDAY, @StartDate) = 1
	RETURN CONVERT(varchar(10), DATEADD(DAY, -6, @StartDate), 23)

IF DATEPART(WEEKDAY, @StartDate) = 2
	RETURN CONVERT(varchar(10), @StartDate, 23)

IF DATEPART(WEEKDAY, @StartDate) = 3
	RETURN CONVERT(varchar(10), DATEADD(DAY, -1, @StartDate), 23)

IF DATEPART(WEEKDAY, @StartDate) = 4
	RETURN CONVERT(varchar(10), DATEADD(DAY, -2, @StartDate), 23)

IF DATEPART(WEEKDAY, @StartDate) = 5
	RETURN CONVERT(varchar(10), DATEADD(DAY, -3, @StartDate), 23)

IF DATEPART(WEEKDAY, @StartDate) = 6
	RETURN CONVERT(varchar(10), DATEADD(DAY, -4, @StartDate), 23)

IF DATEPART(WEEKDAY, @StartDate) = 7
	RETURN CONVERT(varchar(10), DATEADD(DAY, -5, @StartDate), 23)

RETURN CONVERT(varchar(10), GETDATE(), 23)

END