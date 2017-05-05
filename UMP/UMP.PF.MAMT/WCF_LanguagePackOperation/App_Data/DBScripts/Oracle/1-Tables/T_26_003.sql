create table T_26_003
(
  c001 NUMBER(10) not null,
  c002 NUMBER(10) not null,
  c003 NUMBER(5) not null,
  c004 NVARCHAR2(1024),
  c005 NUMBER(5),
  c006 NVARCHAR2(1024),
  c007 NVARCHAR2(1024),
  c008 NVARCHAR2(1024) not null
);
-- Add comments to the table 
comment on table T_26_003
  is '设备表';
-- Add comments to the columns 
comment on column T_26_003.c001
  is 'DEVICEID,设备ID';
comment on column T_26_003.c002
  is '绑定的服务器ID（voiceid），特定值表达为远程，如：-1等';
comment on column T_26_003.c003
  is 'Device类型，0：本地，1：共享，2：FTP，3：其他存储设备';
comment on column T_26_003.c004
  is '服务器（IP地址或机器名）';
comment on column T_26_003.c005
  is '服务器端口';
comment on column T_26_003.c006
  is '用户名';
comment on column T_26_003.c007
  is '密码';
comment on column T_26_003.c008
  is '盘符';
-- Create/Recreate primary, unique and foreign key constraints 
alter table T_26_003
  add constraint PK_26_003 primary key (C001);
