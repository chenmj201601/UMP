create table T_11_002_00000
(
  C001 NUMBER(20) not null,
  C002 VARCHAR2(512) not null,
  C003 VARCHAR2(32) not null,
  C004 VARCHAR2(512) not null,
  C005 VARCHAR2(512) not null,
  C006 NUMBER(20) not null,
  C008 CHAR(1)  default '0' not null,
  C009 VARCHAR2(512),
  C010 CHAR(1),
  C011 VARCHAR2(512),
  C012 NUMBER(5),
  constraint PK_11_002_00000 primary key (C001, C002)
);
comment on table T_11_002_00000
  is '用户登录系统流水表';
-- Add comments to the columns 
comment on column T_11_002_00000.C001
  is '用户编码（11_005.C001）';
comment on column T_11_002_00000.C002
  is '登录时间';
comment on column T_11_002_00000.C003
  is '登录返回状态，默认32个''0''';
comment on column T_11_002_00000.C004
  is '登录机器名';
comment on column T_11_002_00000.C005
  is '登录机器IP';
comment on column T_11_002_00000.C006
  is '登录后分配的SessionID';
comment on column T_11_002_00000.C008
  is '已退出系统，默认''0''';
comment on column T_11_002_00000.C009
  is '退出系统时间。';
comment on column T_11_002_00000.C010
  is '退出系统的方式。';
comment on column T_11_002_00000.C011
  is '最后联系时间';
comment on column T_11_002_00000.C012
  is '登录模块';
create unique index IDX_11_002_001006 on T_11_002_00000 (C001, C006);