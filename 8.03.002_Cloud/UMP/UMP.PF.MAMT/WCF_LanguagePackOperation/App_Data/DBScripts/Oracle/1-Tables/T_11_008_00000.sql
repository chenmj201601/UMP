create table T_11_008_00000
(
  C001 NUMBER(20) not null,
  C002 NUMBER(20) default 0 not null,
  C003 NVARCHAR2(256) not null,
  C004 NVARCHAR2(1024) not null,
  C005 CHAR(1) not null,
  C006 CHAR(1) not null,
  C007 VARCHAR2(32) not null,
  C008 NUMBER(20) not null,
  C009 NUMBER(20) not null,
  C010 DATE not null,
  C011 NVARCHAR2(1024),
constraint PK_11_008_00000 primary key (C001)
);
comment on table T_11_008_00000
  is '技能组列表';
comment on column T_11_008_00000.C001
  is '技能组编号';
comment on column T_11_008_00000.C002
  is '父级技能租编号。目前不使用。默认值 0';
comment on column T_11_008_00000.C003
  is '技能组编码。';
comment on column T_11_008_00000.C004
  is '技能组名称。加密数据';
comment on column T_11_008_00000.C005
  is '是否是活动的。1：活动；0：禁用的';
comment on column T_11_008_00000.C006
  is '已经删除。1：删除；0：未删除';
comment on column T_11_008_00000.C007
  is '其他状态，初始32 个 1';
comment on column T_11_008_00000.C008
  is '所属机构。如果不确定，则 = 0，一般情况下不使用';
comment on column T_11_008_00000.C009
  is '创建人';
comment on column T_11_008_00000.C010
  is '创建时间UTC';
comment on column T_11_008_00000.C011
  is '备注或描述';
create unique index IDX_11_008_C003 on T_11_008_00000 (C003);


