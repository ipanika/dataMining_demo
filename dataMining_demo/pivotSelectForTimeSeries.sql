use DW

SELECT companyID,  [2006] AS '2006', [2007] AS '2007',
								[2008] AS '2008'

FROM 
(SELECT     Company.companyID,   BalanceReport.YearID , 
                      BalanceReport.PeriodEnd
FROM         BalanceReport INNER JOIN
                      BalanceLine ON BalanceReport.balanceLineID = BalanceLine.lineID INNER JOIN
                      Company ON BalanceReport.CompanyID = Company.CompanyID 
					  where BalanceReport.balanceLineID = 636 and 
					  --where 
					  Company.companyID = 150 
					  ) p
PIVOT
(
sum (PeriodEnd)
FOR YearID IN
([2006], [2007], [2008])
) AS pvt --ORDER BY pvt.numLin
