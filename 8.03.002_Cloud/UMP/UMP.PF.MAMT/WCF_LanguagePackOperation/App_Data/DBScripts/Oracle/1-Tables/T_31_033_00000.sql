
create table T_31_033_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 CHAR(1) not null,
  c004 NUMBER(5) not null,
  constraint PK_31_033_0 primary key (C001, C002)
);
comment on table T_31_033_00000
  is '新任务夹临时表，下次分配时先清空';
comment on column T_31_033_00000.c001
  is '录音流水号,对应 对应T_21_000.RecoredReference';
comment on column T_31_033_00000.c002
  is '查询条件ID';
comment on column T_31_033_00000.c003
  is '是否被QM等级锁定';
comment on column T_31_033_00000.c004
  is '质检人员等级ID';
