create table T_00_007
(
  C001 NUMBER(5) not null,
  C002 NUMBER(5) not null,
  C003 NUMBER(5) not null,
  C004 NUMBER(5) not null,
  C005 NUMBER(5) not null,
  C006 NUMBER(5) not null,
  C007 NVARCHAR2(1024),
  C008 NVARCHAR2(1024),
  C009 NVARCHAR2(1024),
  C010 NVARCHAR2(1024),
  constraint PK_00_007 primary key (C001, C002, C003, C004, C005, C006)
);
comment on table T_00_007
  is '告警系统使用语言包';
comment on column T_00_007.C001
  is '语言编码。简体中文：2052；繁体中文：1028；英语（U.S.）：1033';
comment on column T_00_007.C002
  is '告警源模块类型编码';
comment on column T_00_007.C003
  is '告警类型';
comment on column T_00_007.C004
  is '告警消息ID';
comment on column T_00_007.C005
  is '告警子消息ID';
comment on column T_00_007.C006
  is '发送方式';
comment on column T_00_007.C007
  is '消息内容简介';
comment on column T_00_007.C008
  is '消息具体内容1';
comment on column T_00_007.C009
  is '消息具体内容2';
comment on column T_00_007.C010
  is '解决方法';
