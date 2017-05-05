create table T_00_121
(
  C001 NUMBER(20) not null,
  C002 NVARCHAR2(512) not null,
  C011 NVARCHAR2(512) not null,
  C012 NVARCHAR2(512) not null,
  C021 NVARCHAR2(512) not null,
  C022 NVARCHAR2(512) not null,
  constraint PK_00_121 primary key (C001)
);
-- Add comments to the table 
comment on table T_00_121
  is '租户列表';
-- Add comments to the columns 
comment on column T_00_121.C001
  is '租户编码（100开头）';
comment on column T_00_121.C002
  is '租户全名';
comment on column T_00_121.C011
  is '开始租赁时间';
comment on column T_00_121.C012
  is '结束租赁时间';
comment on column T_00_121.C021
  is '租户唯一编码(5位，表名使用)';
comment on column T_00_121.C022
  is '租户登录唯一标识码';
