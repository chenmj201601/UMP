
create table T_31_018_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 CHAR(1),
  c004 NUMBER(20),
  c005 NUMBER(20),
  c006 NUMBER(20)
);
comment on table T_31_018_00000
  is '自定义申诉设置时关联到人';
comment on column T_31_018_00000.c001
  is '对应T_031_017_00000.c001';
comment on column T_31_018_00000.c002
  is '对应T_31_016_00000.c001';
comment on column T_31_018_00000.c003
  is 'S表示给技能组做自定义，O给组织和机构做自定义
';
comment on column T_31_018_00000.c004
  is '该分数创建时该座席所属部门T_11_006_00000.C001';
comment on column T_31_018_00000.c005
  is '技能组编号。T_11_008_00000.C001';
comment on column T_31_018_00000.c006
  is '用户表编号。T_11_005_00000.C001';
alter table T_31_018_00000
  add constraint PK_31_018_0 primary key (C001);
