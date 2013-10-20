/*
   20 октября 2013 г.19:26:20
   User: 
   Server: localhost
   Database: demo_source
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.demo_mstr
	DROP CONSTRAINT PK_demo_mStr
GO
ALTER TABLE dbo.demo_mstr ADD CONSTRAINT
	PK_demo_mstr_1 PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.demo_mstr SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
