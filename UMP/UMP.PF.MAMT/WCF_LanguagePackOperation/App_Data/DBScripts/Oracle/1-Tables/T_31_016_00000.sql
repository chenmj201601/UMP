
create table T_31_016_00000
(
  c001 NUMBER(20) not null,
  c002 CHAR(1) not null,
  c003 CHAR(1) not null,
  c004 NUMBER(20),
  c005 NUMBER(20),
  c006 NUMBER(5),
  c007 CHAR(1),
  c008 CHAR(1),
  c009 CHAR(1)
);
comment on table T_31_016_00000
  is '�ͻ��Զ����������ã��˱��г�ʼ�����ݣ�����Ĭ��ȫ�ֵ�';
comment on column T_31_016_00000.c001
  is '�������������߲���ID';
comment on column T_31_016_00000.c002
  is 'T�������̣�Ҳ�������ó�ʼ����,U�û��Զ�������';
comment on column T_31_016_00000.c003
  is 'Y��ʾ����,N�����';
comment on column T_31_016_00000.c004
  is '�÷�������ʱ����ϯ��������T_11_006_00000.C001';
comment on column T_31_016_00000.c005
  is '�������š�T_11_008_00000.C001';
comment on column T_31_016_00000.c006
  is '���ߴ���(Ĭ��Ϊ1)';
comment on column T_31_016_00000.c007
  is 'S��ʾ�����������Զ��壬O����֯�ͻ������Զ���';
comment on column T_31_016_00000.c008
  is '�Ƿ�ȡ���������� Y�Ƿ�粿�ţ�N���粿��';
comment on column T_31_016_00000.c009
  is 'Yȡ����N��ȡ��';
alter table T_31_016_00000
  add constraint PK_31_016_0 primary key (C001);