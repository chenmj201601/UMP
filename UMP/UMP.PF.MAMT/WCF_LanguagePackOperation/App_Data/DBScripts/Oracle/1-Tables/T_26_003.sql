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
  is '�豸��';
-- Add comments to the columns 
comment on column T_26_003.c001
  is 'DEVICEID,�豸ID';
comment on column T_26_003.c002
  is '�󶨵ķ�����ID��voiceid�����ض�ֵ���ΪԶ�̣��磺-1��';
comment on column T_26_003.c003
  is 'Device���ͣ�0�����أ�1������2��FTP��3�������洢�豸';
comment on column T_26_003.c004
  is '��������IP��ַ���������';
comment on column T_26_003.c005
  is '�������˿�';
comment on column T_26_003.c006
  is '�û���';
comment on column T_26_003.c007
  is '����';
comment on column T_26_003.c008
  is '�̷�';
-- Create/Recreate primary, unique and foreign key constraints 
alter table T_26_003
  add constraint PK_26_003 primary key (C001);
