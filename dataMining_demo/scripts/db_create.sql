USE [master]
GO
/****** Object:  Database [DM]    Script Date: 12.12.2013 23:36:09 ******/
CREATE DATABASE [DM]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DM', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\DM.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'DM_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\DM_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [DM] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DM].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DM] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DM] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DM] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DM] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DM] SET ARITHABORT OFF 
GO
ALTER DATABASE [DM] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DM] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [DM] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DM] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DM] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DM] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DM] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DM] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DM] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DM] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DM] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DM] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DM] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DM] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DM] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DM] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DM] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DM] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DM] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [DM] SET  MULTI_USER 
GO
ALTER DATABASE [DM] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DM] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DM] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DM] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [DM]
GO
/****** Object:  User [NT SERVICE\MSSQLServerOLAPService]    Script Date: 12.12.2013 23:36:09 ******/
CREATE USER [NT SERVICE\MSSQLServerOLAPService] FOR LOGIN [NT SERVICE\MSSQLServerOLAPService] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [NT SERVICE\MSSQLServerOLAPService]
GO
/****** Object:  Table [dbo].[algorithm_variants]    Script Date: 12.12.2013 23:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[algorithm_variants](
	[id_algorithm_variant] [bigint] IDENTITY(1,1) NOT NULL,
	[id_algorithm] [bigint] NULL,
	[name] [nvarchar](max) NULL,
 CONSTRAINT [PK_algorithm_variants] PRIMARY KEY CLUSTERED 
(
	[id_algorithm_variant] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[algorithms]    Script Date: 12.12.2013 23:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[algorithms](
	[id_algorithm] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](max) NULL,
	[task_type] [bigint] NULL,
 CONSTRAINT [PK_algorithms] PRIMARY KEY CLUSTERED 
(
	[id_algorithm] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[data_source_views]    Script Date: 12.12.2013 23:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[data_source_views](
	[id_dsv] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](max) NULL,
	[id_task] [bigint] NULL,
 CONSTRAINT [PK_data_source_views] PRIMARY KEY CLUSTERED 
(
	[id_dsv] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[dsv_columns]    Script Date: 12.12.2013 23:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dsv_columns](
	[id_column] [bigint] IDENTITY(1,1) NOT NULL,
	[id_dsv] [bigint] NULL,
	[column_name] [nvarchar](max) NULL,
 CONSTRAINT [PK_dsv_columns] PRIMARY KEY CLUSTERED 
(
	[id_column] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[models]    Script Date: 12.12.2013 23:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[models](
	[id_model] [bigint] IDENTITY(1,1) NOT NULL,
	[id_structure] [bigint] NULL,
	[name] [nvarchar](max) NULL,
	[id_algorithm_variant] [bigint] NULL,
 CONSTRAINT [PK_models] PRIMARY KEY CLUSTERED 
(
	[id_model] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[parameters]    Script Date: 12.12.2013 23:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[parameters](
	[id_parameter] [bigint] IDENTITY(1,1) NOT NULL,
	[id_algorithm_variant] [bigint] NULL,
	[name] [nvarchar](max) NULL,
	[value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_parameters] PRIMARY KEY CLUSTERED 
(
	[id_parameter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[selection_content]    Script Date: 12.12.2013 23:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[selection_content](
	[id_record] [bigint] IDENTITY(1,1) NOT NULL,
	[id_row] [bigint] NULL,
	[id_column] [bigint] NULL,
	[column_value] [nvarchar](max) NULL,
 CONSTRAINT [PK_selection_content] PRIMARY KEY CLUSTERED 
(
	[id_record] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[selection_rows]    Script Date: 12.12.2013 23:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[selection_rows](
	[id_row] [bigint] IDENTITY(1,1) NOT NULL,
	[id_selection] [bigint] NULL,
 CONSTRAINT [PK_selection_rows] PRIMARY KEY CLUSTERED 
(
	[id_row] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[selections]    Script Date: 12.12.2013 23:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[selections](
	[id_selection] [bigint] IDENTITY(1,1) NOT NULL,
	[id_dsv] [bigint] NULL,
	[name] [nvarchar](max) NULL,
	[filter] [nvarchar](max) NULL,
 CONSTRAINT [PK_data_selections_1] PRIMARY KEY CLUSTERED 
(
	[id_selection] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[structures]    Script Date: 12.12.2013 23:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[structures](
	[id_structure] [bigint] IDENTITY(1,1) NOT NULL,
	[id_selection] [bigint] NULL,
	[name] [nvarchar](max) NULL,
	[test_data_ratio] [int] NULL,
 CONSTRAINT [PK_structures] PRIMARY KEY CLUSTERED 
(
	[id_structure] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tasks]    Script Date: 12.12.2013 23:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tasks](
	[id_task] [bigint] IDENTITY(1,1) NOT NULL,
	[task_type] [bigint] NOT NULL,
	[name] [varchar](max) NULL,
 CONSTRAINT [PK_tasks_1] PRIMARY KEY CLUSTERED 
(
	[id_task] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[algorithm_variants]  WITH CHECK ADD  CONSTRAINT [FK_algorithm_variants_algorithms] FOREIGN KEY([id_algorithm])
REFERENCES [dbo].[algorithms] ([id_algorithm])
GO
ALTER TABLE [dbo].[algorithm_variants] CHECK CONSTRAINT [FK_algorithm_variants_algorithms]
GO
ALTER TABLE [dbo].[data_source_views]  WITH CHECK ADD  CONSTRAINT [FK_data_source_views_tasks] FOREIGN KEY([id_task])
REFERENCES [dbo].[tasks] ([id_task])
GO
ALTER TABLE [dbo].[data_source_views] CHECK CONSTRAINT [FK_data_source_views_tasks]
GO
ALTER TABLE [dbo].[dsv_columns]  WITH CHECK ADD  CONSTRAINT [FK_dsv_columns_data_source_views] FOREIGN KEY([id_dsv])
REFERENCES [dbo].[data_source_views] ([id_dsv])
GO
ALTER TABLE [dbo].[dsv_columns] CHECK CONSTRAINT [FK_dsv_columns_data_source_views]
GO
ALTER TABLE [dbo].[models]  WITH CHECK ADD  CONSTRAINT [FK_models_algorithm_variants] FOREIGN KEY([id_algorithm_variant])
REFERENCES [dbo].[algorithm_variants] ([id_algorithm_variant])
GO
ALTER TABLE [dbo].[models] CHECK CONSTRAINT [FK_models_algorithm_variants]
GO
ALTER TABLE [dbo].[models]  WITH CHECK ADD  CONSTRAINT [FK_models_structures] FOREIGN KEY([id_structure])
REFERENCES [dbo].[structures] ([id_structure])
GO
ALTER TABLE [dbo].[models] CHECK CONSTRAINT [FK_models_structures]
GO
ALTER TABLE [dbo].[parameters]  WITH CHECK ADD  CONSTRAINT [FK_parameters_algorithm_variants] FOREIGN KEY([id_algorithm_variant])
REFERENCES [dbo].[algorithm_variants] ([id_algorithm_variant])
GO
ALTER TABLE [dbo].[parameters] CHECK CONSTRAINT [FK_parameters_algorithm_variants]
GO
ALTER TABLE [dbo].[selection_content]  WITH CHECK ADD  CONSTRAINT [FK_selection_content_dsv_columns] FOREIGN KEY([id_column])
REFERENCES [dbo].[dsv_columns] ([id_column])
GO
ALTER TABLE [dbo].[selection_content] CHECK CONSTRAINT [FK_selection_content_dsv_columns]
GO
ALTER TABLE [dbo].[selection_content]  WITH CHECK ADD  CONSTRAINT [FK_selection_content_selection_rows] FOREIGN KEY([id_row])
REFERENCES [dbo].[selection_rows] ([id_row])
GO
ALTER TABLE [dbo].[selection_content] CHECK CONSTRAINT [FK_selection_content_selection_rows]
GO
ALTER TABLE [dbo].[selection_rows]  WITH CHECK ADD  CONSTRAINT [FK_selection_rows_selections] FOREIGN KEY([id_selection])
REFERENCES [dbo].[selections] ([id_selection])
GO
ALTER TABLE [dbo].[selection_rows] CHECK CONSTRAINT [FK_selection_rows_selections]
GO
ALTER TABLE [dbo].[selections]  WITH CHECK ADD  CONSTRAINT [FK_data_selections_data_source_views] FOREIGN KEY([id_dsv])
REFERENCES [dbo].[data_source_views] ([id_dsv])
GO
ALTER TABLE [dbo].[selections] CHECK CONSTRAINT [FK_data_selections_data_source_views]
GO
ALTER TABLE [dbo].[structures]  WITH CHECK ADD  CONSTRAINT [FK_structures_data_selections] FOREIGN KEY([id_selection])
REFERENCES [dbo].[selections] ([id_selection])
GO
ALTER TABLE [dbo].[structures] CHECK CONSTRAINT [FK_structures_data_selections]
GO
USE [master]
GO
ALTER DATABASE [DM] SET  READ_WRITE 
GO
