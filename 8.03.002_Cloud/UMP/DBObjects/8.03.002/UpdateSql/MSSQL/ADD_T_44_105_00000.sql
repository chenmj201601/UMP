

-- ----------------------------
-- Table structure for T_44_105_00000
-- ----------------------------
CREATE TABLE [dbo].[T_44_105_00000] (
[C001] bigint NOT NULL DEFAULT ((0)) ,
[C002] varchar(1024) NOT NULL ,
[C003] nvarchar(1024) NULL ,
[C004] char(1) NOT NULL DEFAULT ('0') ,
[C005] int NOT NULL DEFAULT ((0)) ,
[C006] nvarchar(1024) NULL ,
[C007] varchar(1024) NULL ,
[C008] varchar(1024) NULL ,
[C009] int NULL DEFAULT ((0)) ,
[C010] char(1) NOT NULL DEFAULT ('0') ,
[C011] bigint NOT NULL DEFAULT ((0)) ,
[C012] bigint NULL ,
[C013] bigint NULL ,
[C014] bigint NULL ,
[C015] bigint NULL 
)


GO

-- ----------------------------
-- Indexes structure for table T_44_105_00000
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table T_44_105_00000
-- ----------------------------
ALTER TABLE [dbo].[T_44_105_00000] ADD PRIMARY KEY ([C001])
GO
