
create table T_31_037_00000
(
  c001 NUMBER(5) not null,
  c002 NUMBER(5) not null,
  c003 VARCHAR2(1024) not null,
  c004 CHAR(1) not null,
  c005 NUMBER(20) not null,
  c006 NUMBER(5) not null,
  constraint PK_31_037_0 primary key (C001)
);
comment on table T_31_037_00000
  is '������,�˱��г�ʼ������';
comment on column T_31_037_00000.c001
  is '��������,��ʼ����д';
comment on column T_31_037_00000.c002
  is '1Ϊ����ʱ�ã�2Ϊ���� 3Ϊ���� 4�Ƽ�¼�� ���Ժ��ж���������';
comment on column T_31_037_00000.c003
  is '��������';
comment on column T_31_037_00000.c004
  is 'YΪ���� NΪ����';
comment on column T_31_037_00000.c005
  is '����ID,T_11_006_00000.C001';
comment on column T_31_037_00000.c006
  is 'ͬ���������';
