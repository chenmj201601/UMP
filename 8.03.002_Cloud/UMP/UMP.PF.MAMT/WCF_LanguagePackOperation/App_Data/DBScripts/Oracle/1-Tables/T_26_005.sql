-- Create table
create table T_26_005
(
  c001 NUMBER(5) not null,
  c002 NUMBER(5) not null,
  c003 NUMBER(11) not null,
  c004 NVARCHAR2(1024) not null,
  c005 NVARCHAR2(1024) not null,
  c006 NUMBER(5) not null,
  c007 NUMBER(5),
  c008 DATE not null
);
-- Add comments to the table 
comment on table T_26_005
  is '远程文件管理';
-- Add comments to the columns 
comment on column T_26_005.c001
  is '归档ID';
comment on column T_26_005.c002
  is '设备ID';
comment on column T_26_005.c003
  is '文件流水号，即录音接口表T_21_001的C001';
comment on column T_26_005.c004
  is '文件路径';
comment on column T_26_005.c005
  is '盘符';
comment on column T_26_005.c006
  is '删除标记:0未删除，1已删除，2已锁定';
comment on column T_26_005.c007
  is '已归档';
comment on column T_26_005.c008
  is '录音启动时间';
-- Create/Recreate primary, unique and foreign key constraints 
alter table T_26_005
  add constraint PK_26_005 primary key (C001, C002, C003);
