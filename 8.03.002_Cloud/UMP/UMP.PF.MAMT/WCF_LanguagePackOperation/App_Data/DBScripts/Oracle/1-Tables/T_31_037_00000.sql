
create table T_31_037_00000
(
  c001 NUMBER(5) not null,
  c002 NUMBER(5) not null,
  c003 VARCHAR2(1024) not null,
  c004 CHAR(1) not null,
  c005 NUMBER(20) not null,
  c006 NUMBER(5) not null,
  constraint PK_31_037_0 primary key (C001)
);
comment on table T_31_037_00000
  is '动作表,此表有初始化数据';
comment on column T_31_037_00000.c001
  is '主键自增,初始化填写';
comment on column T_31_037_00000.c002
  is '1为申诉时用，2为复核 3为审批 4推荐录音 ，以后有多余的再添加';
comment on column T_31_037_00000.c003
  is '动作描述';
comment on column T_31_037_00000.c004
  is 'Y为启用 N为禁用';
comment on column T_31_037_00000.c005
  is '部门ID,T_11_006_00000.C001';
comment on column T_31_037_00000.c006
  is '同类别动作排序';
