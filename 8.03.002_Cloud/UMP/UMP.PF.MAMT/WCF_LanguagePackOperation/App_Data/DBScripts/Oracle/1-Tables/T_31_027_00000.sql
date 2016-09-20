
create table T_31_027_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20),
  c004 NUMBER(5) not null,
  c005 NUMBER(5),
  c006 NUMBER(5) not null
);
comment on table T_31_027_00000
  is '上一个周期剩余表';
comment on column T_31_027_00000.c001
  is '自动任务分配ID,对应 T_31_023_00000.C001';
comment on column T_31_027_00000.c002
  is '座席ID ,对应 T_11_005_00000_0.C001';
comment on column T_31_027_00000.c003
  is '对应上个周期剩下的记录';
comment on column T_31_027_00000.c004
  is '年份';
comment on column T_31_027_00000.c005
  is '月份';
comment on column T_31_027_00000.c006
  is '1代表上旬，2代表中旬 3代表下旬';
alter table T_31_027_00000
  add constraint PK_T_31_027_0 primary key (C001, C002)

