
create table T_31_017_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(5) not null,
  c003 NVARCHAR2(100) not null,
  c004 NUMBER(5) not null,
  c005 NUMBER(5) not null,
  c006 CHAR(1) not null,
  c007 NVARCHAR2(100) not null,
  c008 NVARCHAR2(100) not null,
  c009 NUMBER(5) default -1 not null
);
comment on table T_31_017_00000
  is '申诉处理动作关联表（申诉）';
comment on column T_31_017_00000.c001
  is '自增主键,申诉步骤ID
';
comment on column T_31_017_00000.c002
  is '对应T_31_016_00000.c001';
comment on column T_31_017_00000.c003
  is '自定义步骤名称
';
comment on column T_31_017_00000.c004
  is '1为申诉，2为复核，3为审批
';
comment on column T_31_017_00000.c005
  is '默认为0，复核是(0~3次），当复核有多次时，顺序区别
';
comment on column T_31_017_00000.c006
  is 'Y表示启用,N表禁用（复核可以禁用，申诉和审批均不能禁用）
';
comment on column T_31_017_00000.c007
  is '该申诉步骤对应权限（如果为申诉则有两个权限，一个是自己申诉，一个替他人申诉）
';
comment on column T_31_017_00000.c008
  is '该步骤对应的动作
';
comment on column T_31_017_00000.c009
  is '单位（天）,默认为-1，这个动作的过期时间
';
alter table T_31_017_00000
  add constraint PK_31_017_0 primary key (C001);
