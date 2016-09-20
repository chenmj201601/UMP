
-- ----------------------------
-- Table structure for T_44_101_00000
-- ----------------------------
CREATE TABLE [dbo].[T_44_101_00000] (
[C001] bigint NOT NULL DEFAULT ((0)) ,
[C002] varchar(256) NOT NULL ,
[C003] nvarchar(1024) NULL ,
[C004] bigint NOT NULL DEFAULT ((0)) ,
[C005] char(1) NOT NULL DEFAULT ('0') ,
[C006] char(1) NOT NULL DEFAULT ('1') ,
[C007] smallint NULL DEFAULT ((0)) ,
[C008] smallint NULL DEFAULT ((0)) ,
[C009] varchar(32) NULL ,
[C010] varchar(256) NULL ,
[C011] bigint NULL ,
[C012] bigint NULL ,
[C013] bigint NULL ,
[C014] bigint NULL ,
[C015] char(1) NULL DEFAULT ('0') 
)


GO

-- ----------------------------
-- Indexes structure for table T_44_101_00000
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table T_44_101_00000
-- ----------------------------
ALTER TABLE [dbo].[T_44_101_00000] ADD PRIMARY KEY ([C001])
GO
