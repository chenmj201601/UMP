
create table T_11_007_00000
(
  C001 NUMBER(20) not null,
  C002 NVARCHAR2(256) not null,
  C003 NUMBER(5) not null,
  C004 VARCHAR2(1024) not null,
  C005 VARCHAR2(1024) not null,
  C006 VARCHAR2(1024),
  C007 CHAR(1) not null,
  C008 CHAR(1) not null,
  C009 NUMBER(20) not null,
  C010 DATE not null,
  C011 NVARCHAR2(1024),
  constraint PK_11_007_00000 primary key (C001)
);
comment on table T_11_007_00000
  is '域（工作组）信息';
comment on column T_11_007_00000.C001
  is '域系统编号';
comment on column T_11_007_00000.C002
  is '域名称';
comment on column T_11_007_00000.C003
  is '排序编号';
comment on column T_11_007_00000.C004
  is '拥有浏览该域权限用户。加密数据';
comment on column T_11_007_00000.C005
  is '对应用户的密码。加密数据';
comment on column T_11_007_00000.C006
  is '浏览根目录。加密数据';
comment on column T_11_007_00000.C007
  is '是否是活动的。1：活动；0：禁用的';
comment on column T_11_007_00000.C008
  is '已经删除。1：删除；0：未删除';
comment on column T_11_007_00000.C009
  is '创建人';
comment on column T_11_007_00000.C010
  is '创建时间UTC';
comment on column T_11_007_00000.C011
  is '备注或描述';
create unique index IDX_11_007_C002_00000 on T_11_007_00000 (C002);
