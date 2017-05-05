create table T_00_006
(
  C001 NUMBER(20) not null,
  C002 VARCHAR2(1024),
  C003 VARCHAR2(1024),
  C004 VARCHAR2(1024),
  C005 VARCHAR2(1024),
  C006 VARCHAR2(1024),
  C007 VARCHAR2(1024),
  C008 VARCHAR2(1024),
  C009 VARCHAR2(1024),
  C010 VARCHAR2(1024),
  C011 VARCHAR2(1024),
  C012 VARCHAR2(1024),
  C013 VARCHAR2(1024),
  C014 VARCHAR2(1024),
  C015 VARCHAR2(1024),
  C016 VARCHAR2(1024),
  constraint PK_00_006 primary key (C001)
);
comment on table T_00_006
  is '功能操作依赖关系';
comment on column T_00_006.C001
  is '功能编号或操作编号';
comment on column T_00_006.C002
  is '该功能或操作开启必须包含的其他功能或操作 01';
comment on column T_00_006.C003
  is '该功能或操作开启必须包含的其他功能或操作 02';
comment on column T_00_006.C004
  is '该功能或操作开启必须包含的其他功能或操作 03';
comment on column T_00_006.C005
  is '该功能或操作开启必须包含的其他功能或操作 04';
comment on column T_00_006.C006
  is '该功能或操作开启必须包含的其他功能或操作 05';
comment on column T_00_006.C007
  is '该功能或操作建议开启的其他功能或操作 01';
comment on column T_00_006.C008
  is '该功能或操作建议开启的其他功能或操作 02';
comment on column T_00_006.C009
  is '该功能或操作建议开启的其他功能或操作 03';
comment on column T_00_006.C010
  is '该功能或操作建议开启的其他功能或操作 04';
comment on column T_00_006.C011
  is '该功能或操作建议开启的其他功能或操作 05';
comment on column T_00_006.C012
  is '该功能或操作关闭后必须关闭的其他功能或操作 01';
comment on column T_00_006.C013
  is '该功能或操作关闭后必须关闭的其他功能或操作 02';
comment on column T_00_006.C014
  is '该功能或操作关闭后必须关闭的其他功能或操作 03';
comment on column T_00_006.C015
  is '该功能或操作关闭后必须关闭的其他功能或操作 04';
comment on column T_00_006.C016
  is '该功能或操作关闭后必须关闭的其他功能或操作 05';
