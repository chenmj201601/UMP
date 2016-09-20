create table T_11_005_00000
(
  C001 NUMBER(20) not null,
  C002 NVARCHAR2(1024) not null,
  C003 NVARCHAR2(1024) not null,
  C004 VARCHAR2(1024) not null,
  C005 VARCHAR2(5) not null,
  C006 NUMBER(20) not null,
  C007 CHAR(1) not null,
  C008 CHAR(1) not null,
  C009 CHAR(1) not null,
  C010 CHAR(1) not null,
  C011 CHAR(1) not null,
  C012 VARCHAR2(32) not null,
  C013 VARCHAR2(512) not null,
  C014 NVARCHAR2(256),
  C015 VARCHAR2(128),
  C016 NUMBER(5),
  C017 VARCHAR2(512) not null,
  C018 VARCHAR2(512),
  C019 NUMBER(20) not null,
  C020 VARCHAR2(512) not null,
  C021 DATE,
  C022 DATE,
  C023 DATE,
  C024 NUMBER(5) default 0 not null,
  C025 CHAR(1) default 1 not null,
  C026 VARCHAR2(512),
  constraint PK_11_005_00000 primary key (C001)
);
-- Add comments to the table 
comment on table T_11_005_00000
  is 'ϵͳ�û���.����Ϣ����ϵ��Ϣ��������';
-- Add comments to the columns 
comment on column T_11_005_00000.C001
  is '�û���š�С��100 Ϊϵͳ��ʼ�����ݣ����������κ��޸�';
comment on column T_11_005_00000.C002
  is '��¼�ʺš����Լ��ܷ�ʽ����';
comment on column T_11_005_00000.C003
  is '�û�ȫ�������Լ��ܷ�ʽ����';
comment on column T_11_005_00000.C004
  is '��¼���롣';
comment on column T_11_005_00000.C005
  is '���ܰ汾�ͼ��ܷ�����ǰ��λ��ʾ���ܰ汾������λ��ʾ���ܷ���';
comment on column T_11_005_00000.C006
  is '����������������������';
comment on column T_11_005_00000.C007
  is '�û���Դ��U���ֹ����룻L��LDAP��S��ϵͳ��ʼ������';
comment on column T_11_005_00000.C008
  is '�Ƿ�������0��δ������1������';
comment on column T_11_005_00000.C009
  is '������ʽ��U:�����ý���������L���������˻���ȫ������������¼�������ƣ���';
comment on column T_11_005_00000.C010
  is '�Ƿ���1����ģ�0���ǻ�ġ�Ĭ��ֵ��1��';
comment on column T_11_005_00000.C011
  is '�Ƿ�ɾ����1:���û���ɾ����Ĭ��ֵ��0 ��ʾû��ɾ��';
comment on column T_11_005_00000.C012
  is '����״̬���Ժ����ϵͳ��Ҫ������ж��壩Ĭ�� 32 ��''0''';
comment on column T_11_005_00000.C013
  is '����¼ʱ�䣨UTC����';
comment on column T_11_005_00000.C014
  is '����¼������';
comment on column T_11_005_00000.C015
  is '����¼����IP';
comment on column T_11_005_00000.C016
  is '����¼ģ��';
comment on column T_11_005_00000.C017
  is '�ܹ���¼ϵͳ�Ŀ�ʼʱ�䣨YYYY-MM-DD HH24:MI:SS��ʽ����������';
comment on column T_11_005_00000.C018
  is '�ܹ���¼ϵͳ�Ľ���ʱ��(��� = UNLIMITED�� ��ʾ������) ��������';
comment on column T_11_005_00000.C019
  is '�����ˡ���Ӧ����C001';
comment on column T_11_005_00000.C020
  is '����ʱ�䡣UTC';
comment on column T_11_005_00000.C021
  is '��ְʱ�䡣UTC';
comment on column T_11_005_00000.C022
  is '��ְʱ�䡣UTC';
comment on column T_11_005_00000.C023
  is '����޸�����ʱ�䡣UTC';
comment on column T_11_005_00000.C024
  is '��¼����';
comment on column T_11_005_00000.C025
  is '�Ƿ�Ϊ���û���''1'':���û�';
comment on column T_11_005_00000.C026
  is '����ʱ�� UTC AEC M002��������';
-- Create/Recreate indexes 
create index IDX_11_005_C002_00000 on T_11_005_00000 (C002)