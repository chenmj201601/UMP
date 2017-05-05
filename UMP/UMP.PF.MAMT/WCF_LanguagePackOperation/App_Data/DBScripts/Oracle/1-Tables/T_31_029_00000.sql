
create table T_31_029_00000
(
  c001 NUMBER(5) not null,
  c002 CHAR(1) not null,
  c003 NUMBER(20) default -1 not null,
  c004 NUMBER(20) default -1 not null,
  c005 NVARCHAR2(50) not null,
  c006 NUMBER(5) not null,
  c007 VARCHAR2(200) not null,
  c008 NUMBER(20) not null,
  c009 CHAR(1) not null
);
comment on table T_31_029_00000
  is '座席等级的配置，用于服务统计';
comment on column T_31_029_00000.c001
  is '座席等级ID';
comment on column T_31_029_00000.c002
  is 'T:是以组织机构 S技能组';
comment on column T_31_029_00000.c003
  is '该分数创建时该座席所属部门T_11_006_0000.C001';
comment on column T_31_029_00000.c004
  is '技能组编号。T_11_008_0000.C001';
comment on column T_31_029_00000.c005
  is '等级别名';
comment on column T_31_029_00000.c006
  is '1代表=,2代表>=,3代表>,4代表<=,5代表< 6 between';
comment on column T_31_029_00000.c007
  is '值';
comment on column T_31_029_00000.c008
  is '运行周期ID';
comment on column T_31_029_00000.c009
  is 'Y启用、N禁用';
  
alter table T_31_029_00000
  add constraint PK_31_029_0 primary key (C001)

