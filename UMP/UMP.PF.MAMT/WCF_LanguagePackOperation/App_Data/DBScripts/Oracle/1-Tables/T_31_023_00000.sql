
create table T_31_023_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(5) not null,
  c003 CHAR(1) default 'Y' not null,
  c004 DATE,
  c005 DATE,
  c006 NUMBER(20) not null,
  c007 CHAR(1) not null,
  c008 CHAR(1) default 'N' not null,
  c009 CHAR(1) default 'N' not null,
  c010 VARCHAR2(2000),
  c011 VARCHAR2(2000),
  c012 VARCHAR2(2000),
  c013 NUMBER(20),
  c014 NUMBER(20) default 1 not null,
  c015 NUMBER(20) default 1 not null,
  c016 CHAR(1) default 'N' not null,
  c017 NUMBER(5),
  c018 NUMBER(5),
  c019 NUMBER(5),
  c020 CHAR(1) default 'N' not null,
  c021 NVARCHAR2(50),
  c022 NVARCHAR2(1024),
  c023 CHAR(1)
);
comment on table T_31_023_00000
  is '�Զ���������Լ���������������ñ�';
comment on column T_31_023_00000.c001
  is '��������,�Զ��������ID';
comment on column T_31_023_00000.c002
  is '1 �Զ��������� 2�Զ��������� 3����������䣨�������)';
comment on column T_31_023_00000.c003
  is '����/����  Y/N';
comment on column T_31_023_00000.c004
  is '�����õ����ÿ�ʼʱ��';
comment on column T_31_023_00000.c005
  is '�����õ����ù���ʱ��';
comment on column T_31_023_00000.c006
  is '�����ˣ���Ӧ T_11_005_00000.C001';
comment on column T_31_023_00000.c007
  is '�Ƿ������� Y��������  N�ǹ�������';
comment on column T_31_023_00000.c008
  is '�Ƿ�ƽ������ Yƽ������  N ���˷��͵�һ����¼��';
comment on column T_31_023_00000.c009
  is 'ƽ�����䲻���¼������취  Y���� N���ƽ�����ʼ�������';
comment on column T_31_023_00000.c010
  is '�ʼ���ԱID,�Զ��Ÿ���,��Ϊ������ʱ������û�ʼ���Ա';
comment on column T_31_023_00000.c011
  is '�ʼ���ԱID,�Զ��Ÿ���';
comment on column T_31_023_00000.c012
  is '�ʼ���ԱID,�Զ��Ÿ���';
comment on column T_31_023_00000.c013
  is '�÷�������ʱ�ô�������������T_11_006_00000.C001';
comment on column T_31_023_00000.c014
  is '��ѯ����ID,��Ӧ T_31_024_00000.C001';
comment on column T_31_023_00000.c015
  is '��������ID,��ӦT_31_026_00000.C001';
comment on column T_31_023_00000.c016
  is 'YΪѮ���� N��Ѯ����';
comment on column T_31_023_00000.c017
  is '��Ѯÿ��ϯ��������';
comment on column T_31_023_00000.c018
  is '��Ѯÿ��ϯ��������';
comment on column T_31_023_00000.c019
  is '��Ѯÿ��ϯ��������';
comment on column T_31_023_00000.c020
  is 'YΪ����Ѯ���㣬��Ѯ���� ����Ѯ����ʱ����Ѯ���� NΪ����';
comment on column T_31_023_00000.c021
  is '��������';
comment on column T_31_023_00000.c022
  is '��������';
comment on column T_31_023_00000.c023
  is '��������Ϊ�������ʱ ������������ͬ����м�¼���Ƿ�������ͬ�ļ�¼ Y�ܣ�N����';
alter table T_31_023_00000
  add constraint PK_T_31_023_0 primary key (C001);
