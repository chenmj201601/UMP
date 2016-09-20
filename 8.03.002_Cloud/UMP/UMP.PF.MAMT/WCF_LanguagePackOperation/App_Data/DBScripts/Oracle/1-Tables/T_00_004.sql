create table T_00_004
(
  C001 NUMBER(5) not null,
  C002 NUMBER(5) not null,
  C003 NVARCHAR2(1024),
  C004 VARCHAR2(16) not null,
  C005 CHAR(1) not null,
  C006 CHAR(1) not null,
  constraint PK_00_004 primary key (C001)
);
comment on table T_00_004
  is '系统支持的语言';
comment on column T_00_004.c001
  is '语言编码 2052：简体中文；1028：繁体中文；1033：英语（美国）等';
comment on column T_00_004.c002
  is '排列顺序';
comment on column T_00_004.c003
  is '显示图标';
comment on column T_00_004.c004
  is '当前版本';
comment on column T_00_004.c005
  is '用户自己修改过 ‘0’未修改，‘1’已做修改';
comment on column T_00_004.c006
  is '是否已经支持';
