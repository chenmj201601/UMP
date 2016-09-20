
create table T_31_007_00000
(
  c001 NUMBER(5) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(5) not null,
  c005 NUMBER(10,4) not null,
  c006 NUMBER(10,4) not null,
  c007 CHAR(1) not null,
  c008 NUMBER(5) not null,
  c009 CHAR(1),
  c010 NUMBER(5) not null,
  c011 NUMBER(10,4),
  constraint PK_31_007_0 primary key (C001)
);
comment on table T_31_007_00000
  is '控制项表';
comment on column T_31_007_00000.c001
  is '一个评分标准有多个控制项时，多个控制项的序号，默认从0计数';
comment on column T_31_007_00000.c002
  is '评分表子项表ID,对应T_31_002_00000.c001';
comment on column T_31_007_00000.c003
  is '评分表ID,对应T_31_001_00000.c001';
comment on column T_31_007_00000.c004
  is '1代表=,2代表>=,3代表>,4代表<=,5代表< 6 between
';
comment on column T_31_007_00000.c005
  is '该项得分的值';
comment on column T_31_007_00000.c006
  is '当JugeType为between时，才会存入实际数值';
comment on column T_31_007_00000.c007
  is '仅针对评分标准有分制的情况，针对分制来更改值;1表示针对实际得分，2针对分制得分';
comment on column T_31_007_00000.c008
  is '对应评分表ID,T_31_001_00000.C001或者评分表子项ID,T_31_002_00000.C001';
comment on column T_31_007_00000.c009
  is '1代表评分表,2代表评分表子项';
comment on column T_31_007_00000.c010
  is '1代表=，2代表+,3代表-，4代表*，5代表/,6代表N/A';
comment on column T_31_007_00000.c011
  is '代表更改对象改变的值';
