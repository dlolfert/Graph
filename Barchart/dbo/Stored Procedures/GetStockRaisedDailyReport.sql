


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetStockRaisedDailyReport] 
	
AS
BEGIN
	--IF (SELECT OBJECT_ID('tempdb.dbo.#StockRaised', 'U')) IS NOT NULL
 -- BEGIN
	--DROP TABLE #StockRaised
 -- END
   
  --Select  Ticker, Headline, TimeStampUtc
  --INTO #StockRaised
  --From [Barchart].[dbo].[NewsFeed]
  --Where Headline LIKE '%price target raised to%'
  
  --Select * From #StockRaised

  SELECT Distinct([NewsFeed].[Ticker]), [dbo].[PriceTarget].Headline, [dbo].[PriceTarget].TimeStampUtc
      
  FROM [Barchart].[dbo].[NewsFeed]

  LEFT JOIN [dbo].[PriceTarget] ON NewsFeed.Ticker = [dbo].[PriceTarget].Ticker

  Where [NewsFeed].TimeStampUtc >= DATEADD(DAY, -2, GETUTCDATE())
  And
  ([NewsFeed].Headline LIKE '%Upgraded%' Or [NewsFeed].Headline Like '%price target raised to%')
  Order By [NewsFeed].Ticker, [dbo].[PriceTarget].TimeStampUtc DESC
END