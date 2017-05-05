
create table T_31_028_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(20) not null,
  c005 DATE not null,
  c006 CHAR(1) not null,
  c007 NUMBER(5) not null,
  c008 DATE
);
comment on table T_31_028_00000
  is '快速查询或者默认查询';
comment on column T_31_028_00000.c001
  is '快速查询ID';
comment on column T_31_028_00000.c002
  is '用户ID ,对应 T_11_034_BU.UserID';
comment on column T_31_028_00000.c003
  is '查询条件ID,对应 T_31_024_QueryParam.QueryID';
comment on column T_31_028_00000.c004
  is '设置该查询条件的人，如果是自己设定的则，则对应用户自己ID';
comment on column T_31_028_00000.c005
  is '设置时间';
comment on column T_31_028_00000.c006
  is 'S:自已  O:代表是管理者给他设定的强制使用的查询条件';
comment on column T_31_028_00000.c007
  is '默认为0，有多个的话+1';
comment on column T_31_028_00000.c008
  is '最后一次使用的时间';
alter table T_31_028_00000
  add constraint PK_31_028_0 primary key (C001)

