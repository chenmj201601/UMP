
create table T_31_012_00000
(
  c001 NUMBER(20) not null,
  c002 CHAR(1) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(10,4),
  c005 NVARCHAR2(200),
  c006 NVARCHAR2(200),
  c007 NVARCHAR2(200),
  c008 NVARCHAR2(200),
  c009 NUMBER(10,4),
  c010 NUMBER(10,4)
);
comment on table T_31_012_00000
  is '评分表样式表';
comment on column T_31_012_00000.c001
  is '主键自增ID,评分表样式表主键';
comment on column T_31_012_00000.c002
  is 'T评分表样式，I评分表子项样式,C备注样式';
comment on column T_31_012_00000.c003
  is '评分表ID，T_31_001_00000.C001';
comment on column T_31_012_00000.c004
  is '字体大小';
comment on column T_31_012_00000.c005
  is '字体粗细';
comment on column T_31_012_00000.c006
  is '字体种类';
comment on column T_31_012_00000.c007
  is '前景色';
comment on column T_31_012_00000.c008
  is '背景色';
comment on column T_31_012_00000.c009
  is '高';
comment on column T_31_012_00000.c010
  is '宽';
alter table T_31_012_00000
  add constraint PK_31_012_0 primary key (C001);