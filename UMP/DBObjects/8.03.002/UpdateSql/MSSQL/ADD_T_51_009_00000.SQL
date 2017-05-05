CREATE TABLE [dbo].[T_51_009_00000](
	[C001] [bigint] NOT NULL,
	[C002] [bigint] NOT NULL,
	[C003] [varchar](64) NOT NULL,
	[C004] [varchar](5) NOT NULL,
	[C005] [bigint] NOT NULL,
	[C006] [bigint] NOT NULL,
	[C007] [nvarchar](225) NOT NULL,
	[C008] [nvarchar](255) NOT NULL,
	[C009] [bigint] NOT NULL,
	[C010] [bigint] NOT NULL,
	[C011] [bigint] NOT NULL,
	[C012] [bigint] NOT NULL,
	[C013] [bigint] NOT NULL,
	[C014] [nvarchar](128) NULL,
	[C015] [nvarchar](32) NULL,
	[C016] [nvarchar](128) NULL,
	[C017] [char](1) NULL,
 CONSTRAINT [SYS_C00113154_00000] PRIMARY KEY CLUSTERED 
(
	[C001] ASC,
	[C005] ASC,
	[C007] ASC,
	[C008] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];
