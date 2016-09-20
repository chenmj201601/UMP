

-- ----------------------------
-- Table structure for T_44_102_00000
-- ----------------------------
CREATE TABLE [dbo].[T_44_102_00000] (
[C001] bigint NOT NULL DEFAULT ((0)) ,
[C002] varchar(256) NOT NULL ,
[C003] nvarchar(1024) NULL ,
[C004] char(1) NOT NULL DEFAULT ('0') ,
[C005] smallint NOT NULL DEFAULT ((0)) ,
[C006] bigint NOT NULL DEFAULT ((0)) ,
[C007] bigint NULL ,
[C008] bigint NULL ,
[C009] bigint NULL ,
[C010] bigint NULL ,
[C011] varchar(32) NULL ,
[C012] varchar(32) NULL DEFAULT ('0') ,
[C013] varchar(32) NULL ,
[C014] varchar(512) NULL 
)


GO

-- ----------------------------
-- Indexes structure for table T_44_102_00000
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table T_44_102_00000
-- ----------------------------
ALTER TABLE [dbo].[T_44_102_00000] ADD PRIMARY KEY ([C001])
GO
