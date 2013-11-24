use demo_dm

SELECT [CompanyID], [Name], [YearID], [Нематериальные активы]

FROM 
(select selection_content.id_row, column_value, dsv_columns.column_name from selection_content INNER JOIN 
	selection_rows ON selection_content.id_row = selection_rows.id_row INNER JOIN
	selections ON selection_rows.id_selection = selections.id_selection INNER JOIN
	dsv_columns ON dsv_columns.id_column = selection_content.id_column  INNER JOIN 
	structures ON structures.id_selection = selections.id_selection  INNER JOIN 
	models ON models.id_structure = structures.id_structure AND models.name =  'mod_4'
						) p
PIVOT
(
max(column_value)
FOR column_name IN
([CompanyID], [Name], [YearID], [Нематериальные активы])
) AS pvt -- ORDER BY pvt.CompanyID
