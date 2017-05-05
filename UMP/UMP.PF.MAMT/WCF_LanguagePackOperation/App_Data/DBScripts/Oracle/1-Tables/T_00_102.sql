create table T_00_102
(
  C001 NUMBER(20) not null,
  C002 VARCHAR2(512) not null,
  C003 NUMBER(5) not null,
  C004 NUMBER(5) not null,
  C005 NUMBER(5) not null,
  C006 CHAR(1) not null,
  C007 NUMBER(11) not null,
  C008 CHAR(1) not null,
  C009 NVARCHAR2(256) not null,
  C010 NUMBER(11) not null,
  C011 CHAR(1) not null,
  C012 NUMBER(5) not null,
  C013 VARCHAR2(256),
  C014 NVARCHAR2(1024),
  C015 NVARCHAR2(1024),
  C016 NVARCHAR2(1024),
  C017 NVARCHAR2(1024),
  constraint PK_00_102 primary key (C001, C002)
);
comment on table T_00_102
  is '可视化实体或视图各属性定义';
comment on column T_00_102.C001
  is '实体/视图编号';
comment on column T_00_102.C002
  is '属性名（字段名），加密保存';
comment on column T_00_102.C003
  is '数据类型';
comment on column T_00_102.C004
  is '数据长度';
comment on column T_00_102.C005
  is '小数位数';
comment on column T_00_102.C006
  is '是否可以为空';
comment on column T_00_102.C007
  is '显示宽度';
comment on column T_00_102.C008
  is '对齐方式';
comment on column T_00_102.C009
  is '显示格式，如 yyyy-MM-dd HH:mm:ss | ###.##0.00 或 style的名称(默认的)';
comment on column T_00_102.C010
  is '从基础数据表中读取数据';
comment on column T_00_102.C011
  is '是否显示';
comment on column T_00_102.C012
  is '显示顺序';
comment on column T_00_102.C013
  is 'SourceTable';
comment on column T_00_102.C014
  is 'SourceColumn';
comment on column T_00_102.C015
  is 'DisplayColumn';
comment on column T_00_102.C016
  is 'WhereCondition';
comment on column T_00_102.C017
  is '说明或描述';

