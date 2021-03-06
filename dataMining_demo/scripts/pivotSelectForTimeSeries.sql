use DW

SELECT YearID, [110] AS 'Нематериальные активы',
[120] AS 'Основные средства'
FROM 
(
	SELECT     BalanceReport.companyID, Company.Name, BalanceReport.YearID,  BalanceLine.numLine, 
						  BalanceReport.PeriodEnd
	FROM         BalanceReport INNER JOIN
						  BalanceLine ON BalanceReport.balanceLineID = BalanceLine.lineID INNER JOIN
						  Company ON BalanceReport.CompanyID = Company.CompanyID INNER JOIN
						  Year ON BalanceReport.YearID = Year.YearID 
						  
					  ) p
PIVOT
(
sum (PeriodEnd)
FOR numline IN
([110], [120])
) AS pvt where name = 'ОАО "Мосдачтрест"'--ORDER BY pvt.CompanyID
