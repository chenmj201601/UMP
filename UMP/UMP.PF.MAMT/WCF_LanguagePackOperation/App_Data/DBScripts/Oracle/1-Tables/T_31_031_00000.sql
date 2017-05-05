create table T_31_031_00000
(
  c001 NUMBER(5) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(10,4) not null,
  c004 NUMBER(20),
  c005 NUMBER(20),
constraint PK_31_031_0 primary key (C001, C002)
);
comment on table T_31_031_00000
  is '新任务夹分配百分比';
comment on column T_31_031_00000.c001
  is '质检人员等级ID';
comment on column T_31_031_00000.c002
  is '查询条件ID';
comment on column T_31_031_00000.c003
  is '百分比值(横向控制累加 <=100)
';
comment on column T_31_031_00000.c004
  is '自动任务分配ID';
comment on column T_31_031_00000.c005
  is '新任务夹附加条件 ,默认为-1';
