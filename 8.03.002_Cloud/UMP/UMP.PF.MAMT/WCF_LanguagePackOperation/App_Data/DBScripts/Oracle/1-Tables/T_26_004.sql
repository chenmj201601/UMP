create table T_26_004
(
  c001 NUMBER(10) not null,
  c002 NUMBER(5) not null,
  c003 NUMBER(11) not null,
  c004 NUMBER(5) not null,
  c005 NUMBER(5) not null,
  c006 NUMBER(10) not null,
  c007 NUMBER(5),
  c008 NUMBER(10) not null
);
-- Add comments to the table 
comment on table T_26_004
  is '预处理任务表';
-- Add comments to the columns 
comment on column T_26_004.c001
  is '归档ID，某特别值来表示为本地记录';
comment on column T_26_004.c002
  is '设备ID，暂不用，保留';
comment on column T_26_004.c003
  is '文件流水号，即录音接口表T_21_001的C001';
comment on column T_26_004.c004
  is '任务类型，0回删，1归档，2备份';
comment on column T_26_004.c005
  is '文件类型，0：录音，1：录屏，2：PCM';
comment on column T_26_004.c006
  is '租户ID';
comment on column T_26_004.c007
  is '失败次数';
comment on column T_26_004.c008
  is '执行任务ID号';
-- Create/Recreate primary, unique and foreign key constraints 
alter table T_26_004
  add constraint PK_26_004 primary key (C001, C002, C003);
