
create table T_11_006_00000
(
  C001 NUMBER(20) not null,
  C002 NVARCHAR2(1024) not null,
  C003 NUMBER(5) not null,
  C004 NUMBER(20) not null,
  C005 CHAR(1) not null,
  C006 CHAR(1) not null,
  C007 VARCHAR2(32) not null,
  C008 VARCHAR2(512) not null,
  C009 VARCHAR2(512) not null,
  C010 NUMBER(20) not null,
  C011 DATE not null,
  C012 NVARCHAR2(1024),
  constraint PK_11_006_00000 primary key (C001)
);
comment on table T_11_006_00000
  is '机构部门';
comment on column T_11_006_00000.C001
  is '机构编号。小于100为系统初始化数据';
comment on column T_11_006_00000.C002
  is '机构名称。可使用加密数据保存';
comment on column T_11_006_00000.C003
  is '机构类型。在全局参数中配置后可选择';
comment on column T_11_006_00000.C004
  is '父级机构编号，对应本表C001';
comment on column T_11_006_00000.C005
  is '是否是活动的。1：活动；0：禁用的';
comment on column T_11_006_00000.C006
  is '已经删除。1：删除；0：未删除';
comment on column T_11_006_00000.C007
  is '其他状态。默认值32 个0';
comment on column T_11_006_00000.C008
  is '有效时间开始加密数据';
comment on column T_11_006_00000.C009
  is '结束时间(如果 = UNLIMITED， 表示无限制) 加密数据';
comment on column T_11_006_00000.C010
  is '创建人。';
comment on column T_11_006_00000.C011
  is '创建时间。UTC';
comment on column T_11_006_00000.C012
  is '备注或描述';

