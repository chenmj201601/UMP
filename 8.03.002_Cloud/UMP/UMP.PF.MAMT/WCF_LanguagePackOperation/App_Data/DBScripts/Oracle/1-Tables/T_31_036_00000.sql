
create table T_31_036_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(1024) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(20) not null,
  constraint PK_31_036_0 primary key (C001));
comment on table T_31_036_00000
  is '反馈表，反馈是有反馈权限的人做，只能有查看反馈权限的人能看到反馈';
comment on column T_31_036_00000.c001
  is '主键自增';
comment on column T_31_036_00000.c002
  is '录音流水号,对应 对应T_21_000.RecoredReference';
comment on column T_31_036_00000.c003
  is '反馈内容';
comment on column T_31_036_00000.c004
  is '反馈人ID，对应 T_11_005_00000.C001';
