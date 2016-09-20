
create table T_31_013_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(5) not null,
  c003 NUMBER(10) not null,
  c004 NUMBER(5) not null,
  c005 NUMBER(10,2) not null,
  c006 NUMBER(10,2) not null,
  c007 CHAR(1) not null,
  c008 NUMBER(20) not null,
  c009 NUMBER(5) not null,
  c010 NVARCHAR2(2000)
);
comment on table T_31_013_00000
  is '高级控制项表';
comment on column T_31_013_00000.c001
  is '当该评分表子项有多个控制项时，多个控制项的序号，默认从0计数';
comment on column T_31_013_00000.c002
  is '评分表子项表ID,对应T_31_002_00000.C001';
comment on column T_31_013_00000.c003
  is '评分表ID,对应T_31_001_00000.C001';
comment on column T_31_013_00000.c004
  is '1代表=,2代表>=,3代表>,4代表<=,5代表<';
comment on column T_31_013_00000.c005
  is '该项得分的值';
comment on column T_31_013_00000.c006
  is '当JugeType为between时，才会存入实际数值';
comment on column T_31_013_00000.c007
  is '仅针对评分标准有分制的情况，针对分制来更改值;1表示针对实际得分，2针对分制得分';
comment on column T_31_013_00000.c008
  is '对应评分表ID或者评分表子项ID';
comment on column T_31_013_00000.c009
  is '1代表评分表,2代表评分标准,3代表其它';
comment on column T_31_013_00000.c010
  is '计算公式';
alter table T_31_013_00000
  add constraint PK_31_013_0 primary key (C001);