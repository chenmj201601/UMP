
create table T_31_009_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(10,4),
  c005 CHAR(1),
  c006 CHAR(1),
  c007 CHAR(1),
  c008 NUMBER(10,4) not null
);
comment on table T_31_009_00000
  is '�ɼ����ֱ������';
comment on column T_31_009_00000.c001
  is '���ֱ�ID,��Ӧ T_31_001_00000.C001';
comment on column T_31_009_00000.c002
  is '���ֱ�����ID,��Ӧ T_31_002_00000.C001';
comment on column T_31_009_00000.c003
  is '�ɼ�ID,��Ӧ T_31_008_00000.C001';
comment on column T_31_009_00000.c004
  is '���ֱ�׼��ʵ�ʳɼ�';
comment on column T_31_009_00000.c005
  is '�����ֱ�׼���ʱ���Ƿ�������N/A,Y�������ã�Nû����';
comment on column T_31_009_00000.c006
  is 'Y��ʾ����������ĺ�ɼ���N��ʾû��';
comment on column T_31_009_00000.c007
  is 'Y�������������ת��ǿ��N/A N���';
comment on column T_31_009_00000.c008
  is 'ʵ�ʼ�������ĳɼ�';
alter table T_31_009_00000
  add constraint PK_T_31_009_0 primary key (C001);

