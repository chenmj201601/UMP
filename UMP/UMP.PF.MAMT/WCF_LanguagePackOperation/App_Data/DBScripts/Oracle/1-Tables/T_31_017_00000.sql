
create table T_31_017_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(5) not null,
  c003 NVARCHAR2(100) not null,
  c004 NUMBER(5) not null,
  c005 NUMBER(5) not null,
  c006 CHAR(1) not null,
  c007 NVARCHAR2(100) not null,
  c008 NVARCHAR2(100) not null,
  c009 NUMBER(5) default -1 not null
);
comment on table T_31_017_00000
  is '���ߴ��������������ߣ�';
comment on column T_31_017_00000.c001
  is '��������,���߲���ID
';
comment on column T_31_017_00000.c002
  is '��ӦT_31_016_00000.c001';
comment on column T_31_017_00000.c003
  is '�Զ��岽������
';
comment on column T_31_017_00000.c004
  is '1Ϊ���ߣ�2Ϊ���ˣ�3Ϊ����
';
comment on column T_31_017_00000.c005
  is 'Ĭ��Ϊ0��������(0~3�Σ����������ж��ʱ��˳������
';
comment on column T_31_017_00000.c006
  is 'Y��ʾ����,N����ã����˿��Խ��ã����ߺ����������ܽ��ã�
';
comment on column T_31_017_00000.c007
  is '�����߲����ӦȨ�ޣ����Ϊ������������Ȩ�ޣ�һ�����Լ����ߣ�һ�����������ߣ�
';
comment on column T_31_017_00000.c008
  is '�ò����Ӧ�Ķ���
';
comment on column T_31_017_00000.c009
  is '��λ���죩,Ĭ��Ϊ-1����������Ĺ���ʱ��
';
alter table T_31_017_00000
  add constraint PK_31_017_0 primary key (C001);
