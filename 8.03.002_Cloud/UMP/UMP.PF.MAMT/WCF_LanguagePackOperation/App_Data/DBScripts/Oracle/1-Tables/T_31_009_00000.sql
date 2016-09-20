
create table T_31_009_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(10,4),
  c005 CHAR(1),
  c006 CHAR(1),
  c007 CHAR(1),
  c008 NUMBER(10,4) not null
);
comment on table T_31_009_00000
  is '成绩评分表子项表';
comment on column T_31_009_00000.c001
  is '评分表ID,对应 T_31_001_00000.C001';
comment on column T_31_009_00000.c002
  is '评分表子项ID,对应 T_31_002_00000.C001';
comment on column T_31_009_00000.c003
  is '成绩ID,对应 T_31_008_00000.C001';
comment on column T_31_009_00000.c004
  is '评分标准的实际成绩';
comment on column T_31_009_00000.c005
  is '该评分标准打分时，是否启动了N/A,Y就是启用，N没启用';
comment on column T_31_009_00000.c006
  is 'Y表示被控制项更改后成绩，N表示没有';
comment on column T_31_009_00000.c007
  is 'Y表被控制项或者跳转项强制N/A N表否';
comment on column T_31_009_00000.c008
  is '实际计算出来的成绩';
alter table T_31_009_00000
  add constraint PK_T_31_009_0 primary key (C001);

