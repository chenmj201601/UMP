create table T_00_005
(
  c001 NUMBER(5) not null,
  c002 VARCHAR2(128) not null,
  c003 NUMBER(5) not null,
  c004 NUMBER(5) not null,
  c005 NVARCHAR2(1024),
  c006 NVARCHAR2(1024),
  c007 NVARCHAR2(1024),
  c008 NVARCHAR2(1024),
  c009 NUMBER(5),
  c010 NUMBER(5),
  c011 VARCHAR2(512),
  c012 VARCHAR2(512),
  constraint PK_00_005 primary key (C001, C002)
);
-- Add comments to the table 
comment on table T_00_005
  is 'ϵͳ���԰�';
-- Add comments to the columns 
comment on column T_00_005.c001
  is '���Ա��롣�������ģ�2052���������ģ�1028��Ӣ�U.S.����1033';
comment on column T_00_005.c002
  is '��Ϣ/���ݱ���';
comment on column T_00_005.c003
  is '�̶� Ĭ��ֵ 0';
comment on column T_00_005.c004
  is '���� Ĭ��ֵ 0';
comment on column T_00_005.c005
  is '������ʾ���� 1';
comment on column T_00_005.c006
  is '������ʾ���� 2';
comment on column T_00_005.c007
  is '��ʾ��ʾ���� 1';
comment on column T_00_005.c008
  is '��ʾ��ʾ���� 2';
comment on column T_00_005.c009
  is 'ģ����';
comment on column T_00_005.c010
  is '��ģ����';
comment on column T_00_005.c011
  is '����Frame��Page������';
comment on column T_00_005.c012
  is '��������';
