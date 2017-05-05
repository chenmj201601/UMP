
create table T_31_020_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(64) not null,
  c003 NVARCHAR2(1024),
  c004 NUMBER(5) not null,
  c005 CHAR(1) not null,
  c006 DATE not null,
  c007 NUMBER(20),
  c008 NUMBER(10) not null,
  c009 DATE,
  c010 NUMBER(10) not null,
  c011 DATE,
  c012 NUMBER(20),
  c013 NUMBER(5),
  c014 NVARCHAR2(1024),
  c015 NUMBER(5) not null,
  c016 NUMBER(5) not null,
  c017 CHAR(1) not null,
  c018 DATE
);
comment on table T_31_020_00000
  is '自动任务设置表';
comment on column T_31_020_00000.c001
  is '主键自增，任务号
';
comment on column T_31_020_00000.c002
  is '任务名称
';
comment on column T_31_020_00000.c003
  is '任务描述
';
comment on column T_31_020_00000.c004
  is '任务类别  1 初检手任务 2初检自动任务，2复检手动任务 3复检自动任务 4推荐录音初检 5推荐录音复检  6 QA质检任务（质检但不计入座席成绩） 7智能任务分配
';
comment on column T_31_020_00000.c005
  is '是否共享任务 Y共享任务  N非共享任务
';
comment on column T_31_020_00000.c006
  is '分配时间
';
comment on column T_31_020_00000.c007
  is '分配人ID ,对应 T_11_034_BU.UserID
';
comment on column T_31_020_00000.c008
  is '分配数量
';
comment on column T_31_020_00000.c009
  is '完成时间
';
comment on column T_31_020_00000.c010
  is '完成数量
';
comment on column T_31_020_00000.c011
  is '最后一次修改任务的时间
';
comment on column T_31_020_00000.c012
  is '最后一次修改任务的人员ID,对应 T_11_034_BU.UserID
';
comment on column T_31_020_00000.c013
  is '日期还有多少天到期发通知
';
comment on column T_31_020_00000.c014
  is '通知人
';
comment on column T_31_020_00000.c015
  is '任务所属年
';
comment on column T_31_020_00000.c016
  is '任务所属月
';
comment on column T_31_020_00000.c017
  is 'Y完成 N未完成
';
comment on column T_31_020_00000.c018
  is '任务完成时间
';
alter table T_31_020_00000
  add constraint PK_T_31_020_0 primary key (C001)

