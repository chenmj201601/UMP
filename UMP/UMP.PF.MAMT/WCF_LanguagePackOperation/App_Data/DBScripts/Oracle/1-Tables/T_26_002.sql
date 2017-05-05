-- Create table
create table T_26_002
(
  c001 NUMBER(10) not null,
  c002 NUMBER(5) not null,
  c003 NUMBER(10),
  c004 NVARCHAR2(1024) not null,
  c005 NVARCHAR2(10),
  c006 NUMBER(5) not null,
  c007 VARCHAR2(1024) not null,
  c008 NUMBER(5) not null
);
-- Add comments to the table 
comment on table T_26_002
  is '归档策略';
-- Add comments to the columns 
comment on column T_26_002.c001
  is '归档策略ID';
comment on column T_26_002.c002
  is '此策略是否为备份';
comment on column T_26_002.c003
  is '租户ID';
comment on column T_26_002.c004
  is '路径格式化字符串，以*区分变量字段';
comment on column T_26_002.c005
  is '文件名前缀';
comment on column T_26_002.c006
  is '串行0，并行1';
comment on column T_26_002.c007
  is '绑定的deviceid，以‘;’分隔';
comment on column T_26_002.c008
  is '删除数据库记录';
-- Create/Recreate primary, unique and foreign key constraints 
alter table T_26_002
  add constraint PK_26_002 primary key (C001);
