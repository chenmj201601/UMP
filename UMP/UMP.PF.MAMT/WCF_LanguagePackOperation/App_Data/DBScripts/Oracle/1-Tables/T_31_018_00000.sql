
create table T_31_018_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 CHAR(1),
  c004 NUMBER(20),
  c005 NUMBER(20),
  c006 NUMBER(20)
);
comment on table T_31_018_00000
  is '�Զ�����������ʱ��������';
comment on column T_31_018_00000.c001
  is '��ӦT_031_017_00000.c001';
comment on column T_31_018_00000.c002
  is '��ӦT_31_016_00000.c001';
comment on column T_31_018_00000.c003
  is 'S��ʾ�����������Զ��壬O����֯�ͻ������Զ���
';
comment on column T_31_018_00000.c004
  is '�÷�������ʱ����ϯ��������T_11_006_00000.C001';
comment on column T_31_018_00000.c005
  is '�������š�T_11_008_00000.C001';
comment on column T_31_018_00000.c006
  is '�û����š�T_11_005_00000.C001';
alter table T_31_018_00000
  add constraint PK_31_018_0 primary key (C001);
