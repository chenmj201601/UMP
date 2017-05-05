
create table T_31_010_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(10) not null,
  c004 NUMBER(5) not null,
  c005 NUMBER(10,4)
);
comment on table T_31_010_00000
  is '成绩评分标准子项表';
comment on column T_31_010_00000.c001
  is '评分表子项成绩ID,对应T_31_008_00000.C001';
comment on column T_31_010_00000.c002
  is '评分表标准子项ID,对应T_31_002_00000.C001';
comment on column T_31_010_00000.c003
  is '对应 T_31_001_00000.C001';
comment on column T_31_010_00000.c004
  is '对应T_31_003_00000.C003,排序ID';
comment on column T_31_010_00000.c005
  is '当该评分标准风格为文本时显示的分数';
alter table T_31_010_00000
  add constraint PK_31_010_0 primary key (C001, C002, C003, C004);