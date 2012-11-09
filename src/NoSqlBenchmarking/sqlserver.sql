USE [Benchmark]
GO

/****** Object:  Table [dbo].[Dummy]    Script Date: 11/09/2012 07:00:21 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Dummy]') AND type in (N'U'))
DROP TABLE [dbo].[Dummy]
GO

USE [Benchmark]
GO

/****** Object:  Table [dbo].[Dummy]    Script Date: 11/09/2012 07:00:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Dummy](
	[_Id] [varchar](50) NOT NULL,
	[Blob] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_Dummy] PRIMARY KEY CLUSTERED 
(
	[_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


USE [Benchmark]
GO

/****** Object:  StoredProcedure [dbo].[InsertDummy]    Script Date: 11/08/2012 20:36:16 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertDummy]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertDummy]
GO

USE [Benchmark]
GO

/****** Object:  StoredProcedure [dbo].[InsertDummy]    Script Date: 11/08/2012 20:36:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[InsertDummy] 
	@_Id	VARCHAR(50),
	@Blob	VARBINARY(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO
		DUMMY
		(_Id,	Blob)
	VALUES
		(@_Id,	@Blob)
		
END

GO


USE [Benchmark]
GO

/****** Object:  StoredProcedure [dbo].[InsertDummy]    Script Date: 11/08/2012 20:36:16 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDummy]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDummy]
GO

USE [Benchmark]
GO

/****** Object:  StoredProcedure [dbo].[InsertDummy]    Script Date: 11/08/2012 20:36:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetDummy] 
	@_Id	VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		* 
	FROM
		DUMMY	
	WHERE
		_Id = @_Id
			
END

GO


