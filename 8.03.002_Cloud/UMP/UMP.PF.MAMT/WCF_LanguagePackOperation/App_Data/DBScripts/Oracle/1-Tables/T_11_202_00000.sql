
create table T_11_202_00000
(
  C001 NUMBER(20) not null,
  C002 NUMBER(20) not null,
  C003 CHAR(1) not null,
  C004 CHAR(1) not null,
  C005 CHAR(1) not null,
  C006 NUMBER(20) not null,
  C007 DATE not null,
  C008 DATE not null,
  C009 DATE not null,
  C010 NVARCHAR2(256),
  C011 NUMBER(20),
  C012 NUMBER(20),
  C013 NUMBER(20),
  C014 NUMBER(20),
  C015 NUMBER(20),
  C016 NUMBER(20),
  C017 NUMBER(20),
  C018 NUMBER(20),
  C019 NUMBER(20),
  C020 NUMBER(20),
constraint PK_11_202_00000 primary key (C001, C002)
);
comment on table T_11_202_00000
  is '角色、用户、机构与功能操作的对应关系，含Except关系';
comment on column T_11_202_00000.C001
  is '用户、角色、技能组编号';
comment on column T_11_202_00000.C002
  is '功能编号或操作编号';
comment on column T_11_202_00000.C003
  is '是否可以使用该权限，1-可以，0-不可以，N-未定义';
comment on column T_11_202_00000.C004
  is '该权限是否可以被继续下方分配， 1-可以，0-不可以，N-未定义';
comment on column T_11_202_00000.C005
  is '权限收回时，已分配的下级权限是否级联收回，1-可以，0-不可以，N-未定义';
comment on column T_11_202_00000.C006
  is '最后修改人';
comment on column T_11_202_00000.C007
  is '最后修改日期';
comment on column T_11_202_00000.C008
  is '启用时间';
comment on column T_11_202_00000.C009
  is '禁用时间';
comment on column T_11_202_00000.C010
  is '备用字段';
comment on column T_11_202_00000.C011
  is '哪些管理或控制的资源不包含该权限01';
comment on column T_11_202_00000.C012
  is '哪些管理或控制的资源不包含该权限02';
comment on column T_11_202_00000.C013
  is '哪些管理或控制的资源不包含该权限03';
comment on column T_11_202_00000.C014
  is '哪些管理或控制的资源不包含该权限04';
comment on column T_11_202_00000.C015
  is '哪些管理或控制的资源不包含该权限05';
comment on column T_11_202_00000.C016
  is '哪些管理或控制的资源不包含该权限06';
comment on column T_11_202_00000.C017
  is '哪些管理或控制的资源不包含该权限07';
comment on column T_11_202_00000.C018
  is '哪些管理或控制的资源不包含该权限08';
comment on column T_11_202_00000.C019
  is '哪些管理或控制的资源不包含该权限09';
comment on column T_11_202_00000.C020
  is '哪些管理或控制的资源不包含该权限10';

