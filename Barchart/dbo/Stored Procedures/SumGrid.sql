

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SumGrid] 
	@Symbol AS NVARCHAR(MAX) = 'SPXL'
AS
BEGIN

PRINT @Symbol

DECLARE @100 AS INT
DECLARE @90 AS INT
DECLARE @80 AS INT
DECLARE @70 AS INT
DECLARE @60 AS INT
DECLARE @50 AS INT
DECLARE @40 AS INT
DECLARE @30 AS INT
DECLARE @20 AS INT
DECLARE @10 AS INT
DECLARE @COUNT AS INT

DECLARE @DH100 AS DECIMAL(9,2)
DECLARE @DH90 AS DECIMAL(9,2)
DECLARE @DH80 AS DECIMAL(9,2)
DECLARE @DH70 AS DECIMAL(9,2)
DECLARE @DH60 AS DECIMAL(9,2)
DECLARE @DH50 AS DECIMAL(9,2)
DECLARE @DH40 AS DECIMAL(9,2)
DECLARE @DH30 AS DECIMAL(9,2)
DECLARE @DH20 AS DECIMAL(9,2)
DECLARE @DH10 AS DECIMAL(9,2)

IF OBJECT_ID('tempdb..#Base') IS NOT NULL
    DROP TABLE #Base

CREATE TABLE #Base
(
	ID INT IDENTITY(1, 1) ,
	[Date] DATETIME,
	DayHigh DECIMAL(9,2),
	[Open] DECIMAL(9,2),
	[Close] DECIMAL(9,2),
	V100 DECIMAL(9,2) NULL,
	V90 DECIMAL(9,2) NULL,
	V80 DECIMAL(9,2) NULL,
	V70 DECIMAL(9,2) NULL,
	V60 DECIMAL(9,2) NULL,
	V50 DECIMAL(9,2) NULL,
	V40 DECIMAL(9,2) NULL,
	V30 DECIMAL(9,2) NULL,
	V20 DECIMAL(9,2) NULL,
	V10 DECIMAL(9,2) NULL 
);

Insert Into #Base
SELECT
[Date], 
CASE  
WHEN ([DayHigh] - [Open]) > 0 THEN ([DayHigh] - [Open]) 
WHEN ([DayHigh] - [Open]) <= 0 THEN (([Open] - [Close]) * -1) 
END AS DayHigh,
[Open],
[CLose],
NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL               
FROM [Barchart].[dbo].[Yahoo] Where Symbol = @Symbol And [DATE] >= DATEADD(DAY, -500, GETDATE()) Order By DayHigh




Select @Count = COUNT(*) From #Base 
--PRINT @Count
SET @100 = 1
SET @90 = CAST(@Count * .1 AS INT)
SET @80 = CAST(@Count * .2 AS INT)
SET @70 = CAST(@Count * .3 AS INT)
SET @60 = CAST(@Count * .4 AS INT)
SET @50 = CAST(@Count * .5 AS INT)
SET @40 = CAST(@Count * .6 AS INT)
SET @30 = CAST(@Count * .7 AS INT)
SET @20 = CAST(@Count * .8 AS INT)
SET @10 = CAST(@Count * .9 AS INT)

Select @DH100 = DayHigh From #Base Where ID = @100
Select @DH90 = DayHigh From #Base Where ID = @90
Select @DH80 = DayHigh From #Base Where ID = @80
Select @DH70 = DayHigh From #Base Where ID = @70
Select @DH60 = DayHigh From #Base Where ID = @60
Select @DH50 = DayHigh From #Base Where ID = @50
Select @DH40 = DayHigh From #Base Where ID = @40
Select @DH30 = DayHigh From #Base Where ID = @30
Select @DH20 = DayHigh From #Base Where ID = @20
Select @DH10 = DayHigh From #Base Where ID = @10

DECLARE @ID AS INT
DECLARE @DATE AS DATETIME
DECLARE @DayHigh AS DECIMAL(6,2)
DECLARE @Open AS DECIMAL(6,2)
DECLARE @Close AS DECIMAL(6,2)

DECLARE cbase  CURSOR   
     FOR Select ID, [DATE], DayHigh, [OPen], [close] From #BASE 
	 OPEN cbase;
FETCH NEXT FROM cbase INTO @ID, @DATE, @DayHigh, @Open, @Close;
 
WHILE @@FETCH_STATUS = 0
BEGIN
 IF @DayHigh < 0 
 BEGIN
	UPDATE #Base SET V100 = (DayHigh * 100) Where ID = @ID
	UPDATE #Base SET V90 = (DayHigh * 100) Where ID = @ID
	UPDATE #Base SET V80 = (DayHigh * 100) Where ID = @ID
	UPDATE #Base SET V70 = (DayHigh * 100) Where ID = @ID
	UPDATE #Base SET V60 = (DayHigh * 100) Where ID = @ID
	UPDATE #Base SET V50 = (DayHigh * 100) Where ID = @ID
	UPDATE #Base SET V40 = (DayHigh * 100) Where ID = @ID
	UPDATE #Base SET V30 = (DayHigh * 100) Where ID = @ID
	UPDATE #Base SET V20 = (DayHigh * 100) Where ID = @ID
	UPDATE #Base SET V10 = (DayHigh * 100) Where ID = @ID
 END
 IF @DayHigh = 0
 BEGIN
	UPDATE #Base SET V100 = 0.00 Where ID = @ID
	UPDATE #Base SET V90 = 0.00 Where ID = @ID
	UPDATE #Base SET V80 = 0.00 Where ID = @ID
	UPDATE #Base SET V70 = 0.00 Where ID = @ID
	UPDATE #Base SET V60 = 0.00 Where ID = @ID
	UPDATE #Base SET V50 = 0.00 Where ID = @ID
	UPDATE #Base SET V40 = 0.00 Where ID = @ID
	UPDATE #Base SET V30 = 0.00 Where ID = @ID
	UPDATE #Base SET V20 = 0.00 Where ID = @ID
	UPDATE #Base SET V10 = 0.00 Where ID = @ID
 END
 IF(@DayHigh > 0)
 BEGIN

 UPDATE #Base SET V100 = ((@Close - @Open) * 100) Where ID = @ID
 IF @DayHigh >= @DH100 AND @DH100 > 0 UPDATE #Base SET V100 = (@DH100 * 100) Where ID = @ID
 
 UPDATE #Base SET V90 = ((@Close - @Open) * 100) Where ID = @ID
 IF @DayHigh >= @DH90 AND @DH90 > 0 UPDATE #Base SET V90 = (@DH90 * 100) Where ID = @ID
 
 UPDATE #Base SET V80 = ((@Close - @Open) * 100) Where ID = @ID
 IF @DayHigh >= @DH80 AND @DH80 > 0 UPDATE #Base SET V80 = (@DH80 * 100) Where ID = @ID
 
 UPDATE #Base SET V70 = ((@Close - @Open) * 100) Where ID = @ID
 IF @DayHigh >= @DH70 AND @DH70 > 0 UPDATE #Base SET V70 = (@DH70 * 100) Where ID = @ID
 
 UPDATE #Base SET V60 = ((@Close - @Open) * 100) Where ID = @ID
 IF @DayHigh >= @DH60 AND @DH60 > 0 UPDATE #Base SET V60 = (@DH60 * 100) Where ID = @ID
 
 UPDATE #Base SET V50 = ((@Close - @Open) * 100) Where ID = @ID
 IF @DayHigh >= @DH50 AND @DH50 > 0 UPDATE #Base SET V50 = (@DH50 * 100) Where ID = @ID
 
 UPDATE #Base SET V40 = ((@Close - @Open) * 100) Where ID = @ID
 IF @DayHigh >= @DH40 AND @DH40 > 0 UPDATE #Base SET V40 = (@DH40 * 100) Where ID = @ID

 UPDATE #Base SET V30 = ((@Close - @Open) * 100) Where ID = @ID
 IF @DayHigh >= @DH30 AND @DH30 > 0 UPDATE #Base SET V30 = (@DH30 * 100) Where ID = @ID

 UPDATE #Base SET V20 = ((@Close - @Open) * 100) Where ID = @ID
 IF @DayHigh >= @DH20 AND @DH20 > 0 UPDATE #Base SET V20 = (@DH20 * 100) Where ID = @ID

 UPDATE #Base SET V10 = ((@Close - @Open) * 100) Where ID = @ID
 IF @DayHigh >= @DH10 AND @DH10 > 0 UPDATE #Base SET V10 = (@DH10 * 100) Where ID = @ID
 
 END


 FETCH NEXT FROM cbase INTO  @ID, @DATE, @DayHigh, @Open, @Close;
END
 
CLOSE cbase;
DEALLOCATE cbase;

--Select * From #Base Order By DATE DESC

Select SUM(V100) AS 'T100', SUM(V90) AS 'T90', SUM(V80) AS 'T80', SUM(V70) AS 'T70', SUM(V60) AS 'T60', SUM(V50) AS 'T50', SUM(V40) AS 'T40', SUM(V30) AS 'T30', SUM(V20) AS 'T20', SUM(V10) AS 'T10' From #Base 
END