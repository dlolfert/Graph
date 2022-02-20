-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE UpdateTop100WithNewSymbols 
	
AS
BEGIN
	Insert Into Top100 (Symbol) 
	Select Distinct Symbol From ZacksRank Where Symbol NOT IN (Select Symbol From Top100)
END