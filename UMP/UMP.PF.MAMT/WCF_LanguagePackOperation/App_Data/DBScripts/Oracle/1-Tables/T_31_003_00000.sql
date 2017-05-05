
create table T_31_003_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(5) not null,
  c004 NUMBER(10,4),
  c005 NVARCHAR2(1024) not null,
  c006 CHAR(1),
  constraint PK_31_003_0 primary key (C001)
);
comment on table T_31_003_00000
  is '评分标准子项表';
comment on column T_31_003_00000.c001
  is '评分标准的子项ID,对应 T_31_002_00000.C001';
comment on column T_31_003_00000.c002
  is '评分表ID,对应 T_31_001_00000.C001';
comment on column T_31_003_00000.c003
  is '排序ID';
comment on column T_31_003_00000.c004
  is '评分标准子项的分值
';
comment on column T_31_003_00000.c005
  is '子项内容
';
comment on column T_31_003_00000.c006
  is '为非文本风格时，默认是否被选中';
