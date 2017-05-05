create table T_11_204_00000
(
  C001 NUMBER(20) not null,
  C002 VARCHAR2(32) not null,
  C003 VARCHAR2(1024) not null,
  C004 NUMBER(5) not null,
  C005 NVARCHAR2(64) not null,
  C006 CHAR(1) not null,
  constraint PK_11_204_00000 primary key (C001, C002)
);
comment on table T_11_204_00000
  is '用户、角色、技能组对应的分组、功能列表';
comment on column T_11_204_00000.C001
  is '用户、角色、技能组编号';
comment on column T_11_204_00000.C002
  is 'GroupID，登录后主界面的分组编号';
comment on column T_11_204_00000.C003
  is '包含的功能编号,最多48个功能编号，中间用char(27)分开';
comment on column T_11_204_00000.C004
  is '显示顺序';
comment on column T_11_204_00000.C005
  is '分组名称.如果分组名称 = GroupID，则从语言包中都去对应的名称';
comment on column T_11_204_00000.C006
  is '是否显示分组名称，1:显示；0:不显示';
