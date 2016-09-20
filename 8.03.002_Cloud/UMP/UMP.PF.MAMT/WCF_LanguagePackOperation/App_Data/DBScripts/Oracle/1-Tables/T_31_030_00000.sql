
create table T_31_030_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(100) not null,
  c003 DATE,
  c004 CHAR(1) default 'Y' not null,
  constraint PK_31_030_0 primary key (C001)
);
comment on table T_31_030_00000
  is '质检员等级，用于给质检人员加标志，主要用于新任务夹';
comment on column T_31_030_00000.c001
  is '质检员等级ID，主键自增';
comment on column T_31_030_00000.c002
  is '质检员等级名称';
comment on column T_31_030_00000.c003
  is '创建时间';
comment on column T_31_030_00000.c004
  is 'Y为启用 N为禁用';
