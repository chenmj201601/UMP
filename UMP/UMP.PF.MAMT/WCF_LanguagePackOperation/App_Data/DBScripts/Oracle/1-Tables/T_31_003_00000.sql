
create table T_31_003_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(5) not null,
  c004 NUMBER(10,4),
  c005 NVARCHAR2(1024) not null,
  c006 CHAR(1),
  constraint PK_31_003_0 primary key (C001)
);
comment on table T_31_003_00000
  is '���ֱ�׼�����';
comment on column T_31_003_00000.c001
  is '���ֱ�׼������ID,��Ӧ T_31_002_00000.C001';
comment on column T_31_003_00000.c002
  is '���ֱ�ID,��Ӧ T_31_001_00000.C001';
comment on column T_31_003_00000.c003
  is '����ID';
comment on column T_31_003_00000.c004
  is '���ֱ�׼����ķ�ֵ
';
comment on column T_31_003_00000.c005
  is '��������
';
comment on column T_31_003_00000.c006
  is 'Ϊ���ı����ʱ��Ĭ���Ƿ�ѡ��';
