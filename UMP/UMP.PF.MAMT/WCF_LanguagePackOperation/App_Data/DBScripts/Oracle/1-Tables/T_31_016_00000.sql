
create table T_31_016_00000
(
  c001 NUMBER(20) not null,
  c002 CHAR(1) not null,
  c003 CHAR(1) not null,
  c004 NUMBER(20),
  c005 NUMBER(20),
  c006 NUMBER(5),
  c007 CHAR(1),
  c008 CHAR(1),
  c009 CHAR(1)
);
comment on table T_31_016_00000
  is '客户自定义流程设置，此表有初始化数据，用于默认全局的';
comment on column T_31_016_00000.c001
  is '自增主键，申诉步骤ID';
comment on column T_31_016_00000.c002
  is 'T顶级流程（也就是内置初始化）,U用户自定义流程';
comment on column T_31_016_00000.c003
  is 'Y表示启用,N表禁用';
comment on column T_31_016_00000.c004
  is '该分数创建时该座席所属部门T_11_006_00000.C001';
comment on column T_31_016_00000.c005
  is '技能组编号。T_11_008_00000.C001';
comment on column T_31_016_00000.c006
  is '申诉次数(默认为1)';
comment on column T_31_016_00000.c007
  is 'S表示给技能组做自定义，O给组织和机构做自定义';
comment on column T_31_016_00000.c008
  is '是否取消部门限制 Y是否跨部门，N不跨部门';
comment on column T_31_016_00000.c009
  is 'Y取消，N不取消';
alter table T_31_016_00000
  add constraint PK_31_016_0 primary key (C001);