create table T_00_005
(
  c001 NUMBER(5) not null,
  c002 VARCHAR2(128) not null,
  c003 NUMBER(5) not null,
  c004 NUMBER(5) not null,
  c005 NVARCHAR2(1024),
  c006 NVARCHAR2(1024),
  c007 NVARCHAR2(1024),
  c008 NVARCHAR2(1024),
  c009 NUMBER(5),
  c010 NUMBER(5),
  c011 VARCHAR2(512),
  c012 VARCHAR2(512),
  constraint PK_00_005 primary key (C001, C002)
);
-- Add comments to the table 
comment on table T_00_005
  is '系统语言包';
-- Add comments to the columns 
comment on column T_00_005.c001
  is '语言编码。简体中文：2052；繁体中文：1028；英语（U.S.）：1033';
comment on column T_00_005.c002
  is '消息/内容编码';
comment on column T_00_005.c003
  is '程度 默认值 0';
comment on column T_00_005.c004
  is '级别 默认值 0';
comment on column T_00_005.c005
  is '语言显示内容 1';
comment on column T_00_005.c006
  is '语言显示内容 2';
comment on column T_00_005.c007
  is '提示显示内容 1';
comment on column T_00_005.c008
  is '提示显示内容 2';
comment on column T_00_005.c009
  is '模块编号';
comment on column T_00_005.c010
  is '子模块编号';
comment on column T_00_005.c011
  is '所在Frame或Page、窗口';
comment on column T_00_005.c012
  is '对象名称';
