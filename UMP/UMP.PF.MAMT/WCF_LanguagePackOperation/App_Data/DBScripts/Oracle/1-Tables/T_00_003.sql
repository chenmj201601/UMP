create table T_00_003
(
  C001 NUMBER(11) not null,
  C002 NUMBER(5) not null,
  C003 NUMBER(11) not null,
  C004 CHAR(1) not null,
  C005 NUMBER(5) not null,
  C006 VARCHAR2(1024) not null,
  C007 NVARCHAR2(1024),
  constraint PK_00_003 primary key (C001, C002)
);
comment on table T_00_003
  is '系统基础数据表';
comment on column T_00_003.C001
  is 'ModuleID + 000000';
comment on column T_00_003.C002
  is '在该层次中的排序';
comment on column T_00_003.C003
  is '父级编号，对应该表的C001';
comment on column T_00_003.C004
  is '状态，''1''启用,''0''禁用';
comment on column T_00_003.C005
  is '实际数据是否使用加密保存 0不加密保存，大于 0 加密版本';
comment on column T_00_003.C006
  is '实际数据';
comment on column T_00_003.C007
  is '显示图标';

