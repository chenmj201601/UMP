create table T_11_005_00000
(
  C001 NUMBER(20) not null,
  C002 NVARCHAR2(1024) not null,
  C003 NVARCHAR2(1024) not null,
  C004 VARCHAR2(1024) not null,
  C005 VARCHAR2(5) not null,
  C006 NUMBER(20) not null,
  C007 CHAR(1) not null,
  C008 CHAR(1) not null,
  C009 CHAR(1) not null,
  C010 CHAR(1) not null,
  C011 CHAR(1) not null,
  C012 VARCHAR2(32) not null,
  C013 VARCHAR2(512) not null,
  C014 NVARCHAR2(256),
  C015 VARCHAR2(128),
  C016 NUMBER(5),
  C017 VARCHAR2(512) not null,
  C018 VARCHAR2(512),
  C019 NUMBER(20) not null,
  C020 VARCHAR2(512) not null,
  C021 DATE,
  C022 DATE,
  C023 DATE,
  C024 NUMBER(5) default 0 not null,
  C025 CHAR(1) default 1 not null,
  C026 VARCHAR2(512),
  constraint PK_11_005_00000 primary key (C001)
);
-- Add comments to the table 
comment on table T_11_005_00000
  is '系统用户表.域信息、联系信息单独表保存';
-- Add comments to the columns 
comment on column T_11_005_00000.C001
  is '用户编号。小于100 为系统初始化数据，不允许做任何修改';
comment on column T_11_005_00000.C002
  is '登录帐号。可以加密方式保存';
comment on column T_11_005_00000.C003
  is '用户全名。可以加密方式保存';
comment on column T_11_005_00000.C004
  is '登录密码。';
comment on column T_11_005_00000.C005
  is '加密版本和加密方法。前两位表示加密版本，后三位表示加密方法';
comment on column T_11_005_00000.C006
  is '所属机构，所属机构或部门';
comment on column T_11_005_00000.C007
  is '用户来源，U：手工加入；L：LDAP；S：系统初始化数据';
comment on column T_11_005_00000.C008
  is '是否锁定。0：未锁定；1、锁定';
comment on column T_11_005_00000.C009
  is '锁定方式。U:从配置界面锁定；L：不满足账户安全策略锁定（登录次数控制）；';
comment on column T_11_005_00000.C010
  is '是否活动。1：活动的；0：非活动的。默认值：1。';
comment on column T_11_005_00000.C011
  is '是否删除。1:该用户被删除。默认值：0 表示没被删除';
comment on column T_11_005_00000.C012
  is '其他状态（以后根据系统需要另外进行定义）默认 32 个''0''';
comment on column T_11_005_00000.C013
  is '最后登录时间（UTC）。';
comment on column T_11_005_00000.C014
  is '最后登录机器名';
comment on column T_11_005_00000.C015
  is '最后登录机器IP';
comment on column T_11_005_00000.C016
  is '最后登录模块';
comment on column T_11_005_00000.C017
  is '能够登录系统的开始时间（YYYY-MM-DD HH24:MI:SS格式）加密数据';
comment on column T_11_005_00000.C018
  is '能够登录系统的结束时间(如果 = UNLIMITED， 表示无限制) 加密数据';
comment on column T_11_005_00000.C019
  is '创建人。对应本表C001';
comment on column T_11_005_00000.C020
  is '创建时间。UTC';
comment on column T_11_005_00000.C021
  is '入职时间。UTC';
comment on column T_11_005_00000.C022
  is '离职时间。UTC';
comment on column T_11_005_00000.C023
  is '最后修改密码时间。UTC';
comment on column T_11_005_00000.C024
  is '登录次数';
comment on column T_11_005_00000.C025
  is '是否为新用户，''1'':新用户';
comment on column T_11_005_00000.C026
  is '锁定时间 UTC AEC M002方法加密';
-- Create/Recreate indexes 
create index IDX_11_005_C002_00000 on T_11_005_00000 (C002)