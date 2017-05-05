-- Create table
create table T_26_002
(
  c001 NUMBER(10) not null,
  c002 NUMBER(5) not null,
  c003 NUMBER(10),
  c004 NVARCHAR2(1024) not null,
  c005 NVARCHAR2(10),
  c006 NUMBER(5) not null,
  c007 VARCHAR2(1024) not null,
  c008 NUMBER(5) not null
);
-- Add comments to the table 
comment on table T_26_002
  is '�鵵����';
-- Add comments to the columns 
comment on column T_26_002.c001
  is '�鵵����ID';
comment on column T_26_002.c002
  is '�˲����Ƿ�Ϊ����';
comment on column T_26_002.c003
  is '�⻧ID';
comment on column T_26_002.c004
  is '·����ʽ���ַ�������*���ֱ����ֶ�';
comment on column T_26_002.c005
  is '�ļ���ǰ׺';
comment on column T_26_002.c006
  is '����0������1';
comment on column T_26_002.c007
  is '�󶨵�deviceid���ԡ�;���ָ�';
comment on column T_26_002.c008
  is 'ɾ�����ݿ��¼';
-- Create/Recreate primary, unique and foreign key constraints 
alter table T_26_002
  add constraint PK_26_002 primary key (C001);
