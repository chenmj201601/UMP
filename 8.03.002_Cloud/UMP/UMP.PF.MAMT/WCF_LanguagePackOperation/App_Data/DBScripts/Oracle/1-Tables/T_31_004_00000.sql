
create table T_31_004_00000
(
  c001 NUMBER(20) default 0 not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20),
  c004 NUMBER(5) not null,
  c005 NVARCHAR2(2000),
  c006 NUMBER(5),
  c007 NUMBER(5),
  c008 CHAR(1) not null,
  c009 NVARCHAR2(2000),
  c010 CHAR(1) not null,
  constraint PK_31_004_0 primary key (C001)
);
comment on table PT_31_004_00000
  is '��ע��';
comment on column PT_31_004_00000.c001
  is '��������ID,���۱�עID';
comment on column PT_31_004_00000.c002
  is '���ֱ�����ID,��Ӧ T_31_002_00000.c001';
comment on column PT_31_004_00000.c003
  is '���ֱ�ID,��ӦT_31_001_00000.c001';
comment on column PT_31_004_00000.c004
  is '��һ�������ж����עʱ����ע��˳��';
comment on column PT_31_004_00000.c005
  is 'Ĭ���ı����ڵ�����';
comment on column PT_31_004_00000.c006
  is 'Ϊ�ı����ʱ����ע���ֽڳ���,���ֵΪ2000';
comment on column PT_31_004_00000.c007
  is 'Ϊ�ı����ʱ����ע������С���ֽڳ���';
comment on column PT_31_004_00000.c008
  is '1Ϊ�ı���� 2Ϊ������� 3Ϊ��ѡ��� 4Ϊ��ѡ���';
comment on column PT_31_004_00000.c009
  is '���۱�ע����';
comment on column PT_31_004_00000.c010
  is ' YΪ���ֱ�����۱�ע��NΪ���ֱ���������۱�ע';
