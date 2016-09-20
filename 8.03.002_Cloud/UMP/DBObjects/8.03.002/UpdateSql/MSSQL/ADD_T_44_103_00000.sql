
-- ----------------------------
-- Table structure for T_44_103_00000
-- ----------------------------
CREATE TABLE [dbo].[T_44_103_00000] (
[C001] bigint NOT NULL DEFAULT ((0)) ,
[C002] smallint NOT NULL DEFAULT ((0)) ,
[C003] varchar(1024) NOT NULL ,
[C004] nvarchar(1024) NULL ,
[C005] char(1) NOT NULL DEFAULT ('1') ,
[C006] varchar(1024) NOT NULL ,
[C007] varchar(1024) NULL ,
[C008] char(1) NULL DEFAULT ('1') ,
[C009] smallint NOT NULL DEFAULT ((0)) ,
[C010] bigint NOT NULL DEFAULT ((0)) ,
[C011] bigint NULL ,
[C012] bigint NULL ,
[C013] bigint NULL ,
[C014] bigint NULL 
)


GO

-- ----------------------------
-- Indexes structure for table T_44_103_00000
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table T_44_103_00000
-- ----------------------------
ALTER TABLE [dbo].[T_44_103_00000] ADD PRIMARY KEY ([C001])
GO
