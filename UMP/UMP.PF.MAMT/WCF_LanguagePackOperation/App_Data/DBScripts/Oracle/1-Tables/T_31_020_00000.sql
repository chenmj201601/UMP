
create table T_31_020_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(64) not null,
  c003 NVARCHAR2(1024),
  c004 NUMBER(5) not null,
  c005 CHAR(1) not null,
  c006 DATE not null,
  c007 NUMBER(20),
  c008 NUMBER(10) not null,
  c009 DATE,
  c010 NUMBER(10) not null,
  c011 DATE,
  c012 NUMBER(20),
  c013 NUMBER(5),
  c014 NVARCHAR2(1024),
  c015 NUMBER(5) not null,
  c016 NUMBER(5) not null,
  c017 CHAR(1) not null,
  c018 DATE
);
comment on table T_31_020_00000
  is '�Զ��������ñ�';
comment on column T_31_020_00000.c001
  is '���������������
';
comment on column T_31_020_00000.c002
  is '��������
';
comment on column T_31_020_00000.c003
  is '��������
';
comment on column T_31_020_00000.c004
  is '�������  1 ���������� 2�����Զ�����2�����ֶ����� 3�����Զ����� 4�Ƽ�¼������ 5�Ƽ�¼������  6 QA�ʼ������ʼ쵫��������ϯ�ɼ��� 7�����������
';
comment on column T_31_020_00000.c005
  is '�Ƿ������� Y��������  N�ǹ�������
';
comment on column T_31_020_00000.c006
  is '����ʱ��
';
comment on column T_31_020_00000.c007
  is '������ID ,��Ӧ T_11_034_BU.UserID
';
comment on column T_31_020_00000.c008
  is '��������
';
comment on column T_31_020_00000.c009
  is '���ʱ��
';
comment on column T_31_020_00000.c010
  is '�������
';
comment on column T_31_020_00000.c011
  is '���һ���޸������ʱ��
';
comment on column T_31_020_00000.c012
  is '���һ���޸��������ԱID,��Ӧ T_11_034_BU.UserID
';
comment on column T_31_020_00000.c013
  is '���ڻ��ж����쵽�ڷ�֪ͨ
';
comment on column T_31_020_00000.c014
  is '֪ͨ��
';
comment on column T_31_020_00000.c015
  is '����������
';
comment on column T_31_020_00000.c016
  is '����������
';
comment on column T_31_020_00000.c017
  is 'Y��� Nδ���
';
comment on column T_31_020_00000.c018
  is '�������ʱ��
';
alter table T_31_020_00000
  add constraint PK_T_31_020_0 primary key (C001)

