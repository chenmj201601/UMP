-- Create table
create table T_26_005
(
  c001 NUMBER(5) not null,
  c002 NUMBER(5) not null,
  c003 NUMBER(11) not null,
  c004 NVARCHAR2(1024) not null,
  c005 NVARCHAR2(1024) not null,
  c006 NUMBER(5) not null,
  c007 NUMBER(5),
  c008 DATE not null
);
-- Add comments to the table 
comment on table T_26_005
  is 'Զ���ļ�����';
-- Add comments to the columns 
comment on column T_26_005.c001
  is '�鵵ID';
comment on column T_26_005.c002
  is '�豸ID';
comment on column T_26_005.c003
  is '�ļ���ˮ�ţ���¼���ӿڱ�T_21_001��C001';
comment on column T_26_005.c004
  is '�ļ�·��';
comment on column T_26_005.c005
  is '�̷�';
comment on column T_26_005.c006
  is 'ɾ�����:0δɾ����1��ɾ����2������';
comment on column T_26_005.c007
  is '�ѹ鵵';
comment on column T_26_005.c008
  is '¼������ʱ��';
-- Create/Recreate primary, unique and foreign key constraints 
alter table T_26_005
  add constraint PK_26_005 primary key (C001, C002, C003);
