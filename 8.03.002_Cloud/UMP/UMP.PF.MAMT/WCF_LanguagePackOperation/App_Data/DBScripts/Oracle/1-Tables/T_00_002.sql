create table T_00_002
(
  C000 VARCHAR2(5) not null,
  C001 NUMBER(20) not null,
  C002 NUMBER(5) not null,
  C003 VARCHAR2(128) not null,
  C004 DATE not null,
  C005 NVARCHAR2(1024) not null,
  C006 NVARCHAR2(1024) not null,
  C008 NUMBER(5) not null,
  C009 NUMBER(20) not null,
  constraint PK_00_002 primary key (C000, C001, C003, C004)
);
comment on table T_00_002
  is '系统数据的关键数据变更记录信息';
comment on column T_00_002.C000
  is '租户编号. 默认值 00000';
comment on column T_00_002.C001
  is '对象编号';
comment on column T_00_002.C002
  is '对象类型，如用户、角色、租户等';
comment on column T_00_002.C003
  is '变更对象';
comment on column T_00_002.C004
  is '变更时间 UTC';
comment on column T_00_002.C005
  is '变更前数据';
comment on column T_00_002.C006
  is '变更后数据';
comment on column T_00_002.C008
  is '数据是否加密，及加密版本。0为不加密';
comment on column T_00_002.C009
  is '变更人。0：系统自动变更';
