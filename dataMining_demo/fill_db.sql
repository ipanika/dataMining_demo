USE [DM]
GO

INSERT INTO [dbo].[algorithms]
           ([name]
           ,[task_type])
     VALUES
           ('Microsoft_Clustering',1),
		   ('Microsoft_Time_Series', 2)
GO

INSERT INTO [dbo].[tasks]
           ([task_type]
           ,[name])
     VALUES
           (1, '�������������', null, null, null, null), 
		   (2, '���������������', null, null, null, null)
GO