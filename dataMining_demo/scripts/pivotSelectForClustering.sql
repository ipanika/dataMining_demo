use DW

SELECT CompanyID, Name, YearID, [110] AS '�������������� ������',
[120] AS '�������� ��������', [130] AS'������������� �������������',
[135] AS '�������� �������� � ��������.��������',
[140] AS '������������ ���������� ��������',
[145] AS '���������� ��������� ������', 
[150] AS '������ ������������ ������',
[210] AS '������', [211] AS '� �.�. ����� � ���������', 
[212] AS '�������� �� ����������� � �������',
[213] AS '������� � ���', [214] AS '������� ��������� � ������', 
[215] AS '������ �����������', [216] AS '���', [217] AS '������ ������ � �������', [220] AS '���', [230] AS '�� ������������', [240] AS '�� �������������',
[241] AS '���������� � ���������'

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
