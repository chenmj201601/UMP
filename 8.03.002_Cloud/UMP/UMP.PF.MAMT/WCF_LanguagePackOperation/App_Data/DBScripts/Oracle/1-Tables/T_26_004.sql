create table T_26_004
(
  c001 NUMBER(10) not null,
  c002 NUMBER(5) not null,
  c003 NUMBER(11) not null,
  c004 NUMBER(5) not null,
  c005 NUMBER(5) not null,
  c006 NUMBER(10) not null,
  c007 NUMBER(5),
  c008 NUMBER(10) not null
);
-- Add comments to the table 
comment on table T_26_004
  is 'Ԥ���������';
-- Add comments to the columns 
comment on column T_26_004.c001
  is '�鵵ID��ĳ�ر�ֵ����ʾΪ���ؼ�¼';
comment on column T_26_004.c002
  is '�豸ID���ݲ��ã�����';
comment on column T_26_004.c003
  is '�ļ���ˮ�ţ���¼���ӿڱ�T_21_001��C001';
comment on column T_26_004.c004
  is '�������ͣ�0��ɾ��1�鵵��2����';
comment on column T_26_004.c005
  is '�ļ����ͣ�0��¼����1��¼����2��PCM';
comment on column T_26_004.c006
  is '�⻧ID';
comment on column T_26_004.c007
  is 'ʧ�ܴ���';
comment on column T_26_004.c008
  is 'ִ������ID��';
-- Create/Recreate primary, unique and foreign key constraints 
alter table T_26_004
  add constraint PK_26_004 primary key (C001, C002, C003);
