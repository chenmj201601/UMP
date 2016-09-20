create table T_31_034_00000
(
  c001 NUMBER(20) not null,
  c002 CHAR(1) default 1 not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(20) not null,
  c005 NUMBER(10,4) not null,
  c006 DATE,
  c007 NUMBER(5),
  c008 NUMBER(5),
  constraint PK_31_034_0 primary key (C001)
);
comment on table T_31_034_00000
  is '时长趋势历史表';
comment on column T_31_034_00000.c001
  is '主键自增';
comment on column T_31_034_00000.c002
  is 'T：近部门来进行统计，S按技能组进行统计';
comment on column T_31_034_00000.c003
  is '技能组编号。T_11_008_00000.c001';
comment on column T_31_034_00000.c004
  is '该分数创建时该座席所属部门T_11_006_00000.c001';
comment on column T_31_034_00000.c005
  is '时长趋势的值';
comment on column T_31_034_00000.c006
  is '最后一次统计的值';
comment on column T_31_034_00000.c007
  is '统计数据属于年份';
comment on column T_31_034_00000.c008
  is '统计数据属于月份';
