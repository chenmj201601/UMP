
create table T_31_035_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) default 1 not null,
  c004 NUMBER(20) default 1 not null,
  c005 CHAR(1) not null,
  c006 NUMBER(10,4) not null,
  c007 NUMBER(5),
  c008 CHAR(1),
  constraint PK_31_035_0 primary key (C001)
  );
comment on table T_31_035_00000
  is '时长趋势配置表，按部门按技能组统计';
comment on column T_31_035_00000.c001
  is '设置时长趋势ID,主键自增';
comment on column T_31_035_00000.c002
  is '运行周期ID';
comment on column T_31_035_00000.c003
  is '技能组编号。T_11_008_00000.C001';
comment on column T_31_035_00000.c004
  is '该分数创建时该座席所属部门T_11_006_00000.C001';
comment on column T_31_035_00000.c005
  is 'T：近部门来进行统计，S按技能组进行统计';
comment on column T_31_035_00000.c006
  is '预设值';
comment on column T_31_035_00000.c007
  is '1代表=,2代表>=,3代表>,4代表<=,5代表< 6 between';
comment on column T_31_035_00000.c008
  is 'Y启用、N禁用';
