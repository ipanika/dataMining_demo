use DW

SELECT CompanyID, Name, YearID, [110] AS 'Нематериальные активы',
[120] AS 'Основные средства', [130] AS'Незавершенное строительство',
[135] AS 'Доходные вложения в материал.ценности',
[140] AS 'Долгосрочные финансовые вложения',
[145] AS 'Отложенные налоговые активы', 
[150] AS 'Прочие внеоборотные активы',
[210] AS 'Запасы', [211] AS 'в т.ч. сырье и материалы', 
[212] AS 'животные на выращивание и откорме',
[213] AS 'затраты в НЗП', [214] AS 'готовая продукция и товары', 
[215] AS 'товары отгруженные', [216] AS 'РБП', [217] AS 'прочие запасы и затраты', [220] AS 'НДС', [230] AS 'ДЗ долгосрочная', [240] AS 'ДЗ краткосрочная',
[241] AS 'покупатели и заказчики'

--INTO ForDataMining2
FROM 
(SELECT     BalanceReport.companyID, Company.Name, BalanceReport.YearID,  BalanceLine.numLine, 
                      BalanceReport.PeriodEnd
FROM         BalanceReport INNER JOIN
                      BalanceLine ON BalanceReport.balanceLineID = BalanceLine.lineID INNER JOIN
                      Company ON BalanceReport.CompanyID = Company.CompanyID INNER JOIN
                      Year ON BalanceReport.YearID = Year.YearID) p
PIVOT
(
sum (PeriodEnd)
FOR numline IN
([110],[120],[130],[135],[140],[145],[150],[210],[211],[212],[213],[214],[215],
[216],[217],[220],[230],[240],[241])
) AS pvt ORDER BY pvt.CompanyID
