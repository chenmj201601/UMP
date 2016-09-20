
create table T_31_006_00000
(
  c001 NUMBER(5) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(5) not null,
  c004 NUMBER(20),
  constraint PK_31_006_0 primary key (C001, C002, C003)
);
comment on table T_31_006_00000
  is '评分标准子项跳转表,根据评分标准子项表选项那些评分表子项要跳过（即设成N/A）';
comment on column T_31_006_00000.c001
  is '评分表子项ID ，T_31_002_00000.c001';
comment on column T_31_006_00000.c002
  is '评分表ID ，T_31_001_00000.c001';
comment on column T_31_006_00000.c003
  is ' 顺序ID，从1开始';
comment on column T_31_006_00000.c004
  is '评分表子项ID ，T_31_002_00000.c001';
