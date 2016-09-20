
create table T_11_004_00000
(
  C001 NUMBER(20) not null,
  C002 NUMBER(20) not null,
  C003 NUMBER(5) not null,
  C004 NVARCHAR2(1024) not null,
  C005 CHAR(1) not null,
  C006 CHAR(1) not null,
  C007 VARCHAR2(32) not null,
  C008 VARCHAR2(512) not null,
  C009 VARCHAR2(512) not null,
  C010 NUMBER(20) not null,
  C011 DATE not null,
  constraint PK_T_11_004_00000 primary key (C001)
);
comment on table T_11_004_00000
  is '系统角色列表，含默认角色和用户自定义角色';
comment on column T_11_004_00000.C001
  is '角色编号。小于 100 为系统初始化角色，不可进行任何编辑';
comment on column T_11_004_00000.C002
  is '父级角色编号。对应本表C001。如果该值为 0 ，则表示没有父级角色。';
comment on column T_11_004_00000.C003
  is '应用于哪个模块。如果该值为0，表示多模块使用';
comment on column T_11_004_00000.C004
  is '角色名称（加密保存）';
comment on column T_11_004_00000.C005
  is '是否是活动的。1：活动；0：禁用的';
comment on column T_11_004_00000.C006
  is '已经删除。1：删除；0：未删除';
comment on column T_11_004_00000.C007
  is '角色其他状态，默认 32 个0';
comment on column T_11_004_00000.C008
  is '启用时间,加密数据保存。YYYY-MM-DD HH24:MI:SS';
comment on column T_11_004_00000.C009
  is '过期时间，加密数据保存。YYYY-MM-DD HH24:MI:SS。2199-12-31 23：59：59表示永不过期';
comment on column T_11_004_00000.C010
  is '创建人。0表示初始化数据';
comment on column T_11_004_00000.C011
  is '创建时间（UTC）';
