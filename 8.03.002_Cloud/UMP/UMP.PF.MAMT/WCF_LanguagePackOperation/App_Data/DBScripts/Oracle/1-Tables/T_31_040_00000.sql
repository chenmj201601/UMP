
create table T_31_040_00000
(
  c001 VARCHAR2(128) not null,
  c002 NUMBER(5) default 0,
  c003 NUMBER(5) not null,
  c004 NVARCHAR2(1000) default -1,
  c005 NUMBER(5) default -1 not null,
  c006 CHAR(1) not null,
  c007 NUMBER(20) default -1 not null,
  c008 NUMBER(5) default -1 not null,
  constraint PK_31_040_0 primary key (C001, C007)
);
comment on table T_31_040_00000
  is '自定义查询界面';
comment on column T_31_040_00000.c001
  is '查询控件名';
comment on column T_31_040_00000.c002
  is ' 1为textbox 2 combox 3为grid';
comment on column T_31_040_00000.c003
  is '处于tabitem索引的值,从0开始';
comment on column T_31_040_00000.c004
  is '新添加TabItem的名称';
comment on column T_31_040_00000.c005
  is '在一个tabitem里的顺序';
comment on column T_31_040_00000.c006
  is 'Y为系统定义默认的 N为用户个人定义的';
comment on column T_31_040_00000.c007
  is '用户ID ,对应 T_11_005_00000.C001';
comment on column T_31_040_00000.c008
  is '控件高度';

