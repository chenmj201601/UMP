create table T_26_001
(
  c001 NUMBER(5) not null,
  c002 NUMBER(5) not null,
  c003 NUMBER(5) not null,
  c004 NUMBER(5)
);
-- Add comments to the table 
comment on table T_26_001
  is '参数表';
-- Add comments to the columns 
comment on column T_26_001.c001
  is '服务ID号，例：261000以上为只做备份功能';
comment on column T_26_001.c002
  is '是否启用备份功能';
comment on column T_26_001.c003
  is '租户ID';
comment on column T_26_001.c004
  is '此服务是否管理远程设备，1为管理。整套系统唯一';
-- Create/Recreate primary, unique and foreign key constraints 
alter table T_26_001
  add constraint PK_26_001 primary key (C001);
