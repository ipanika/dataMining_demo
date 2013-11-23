use demo_dm

SELECT [CompanyID], [Name], [YearID], [Неметериальные активы], [Основные средства], [Незавершенное строительство]

FROM 
(select id_record, column_value, dsv_columns.column_name from selection_content INNER JOIN 
	selection_rows ON selection_content.id_row = selection_rows.id_row INNER JOIN
	selections ON selection_rows.id_selection = selections.id_selection INNER JOIN
	dsv_columns ON dsv_columns.id_column = selection_content.id_column ) p
PIVOT
(
max(column_value)
FOR column_name IN
([CompanyID], [Name], [YearID], [Неметериальные активы], [Основные средства], [Незавершенное строительство])
) AS pvt -- ORDER BY pvt.CompanyID

select column_value, dsv_columns.column_name from selection_content INNER JOIN 
	selection_rows ON selection_content.id_row = selection_rows.id_row INNER JOIN
	selections ON selection_rows.id_selection = selections.id_selection INNER JOIN
	dsv_columns ON dsv_columns.id_column = selection_content.id_column 