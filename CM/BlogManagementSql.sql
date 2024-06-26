USE [CM]
GO
/****** Object:  Table [dbo].[tree]    Script Date: 24.04.2024 19:10:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tree](
	[ID] [int] NOT NULL,
	[name] [varchar](50) NULL,
	[parent] [int] NULL,
	[HTMLContent] [text] NULL,
	[childcount] [int] NULL,
	[HasContent] [bit] NULL,
 CONSTRAINT [PK_tree] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
