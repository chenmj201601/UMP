
create table T_31_010_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(10) not null,
  c004 NUMBER(5) not null,
  c005 NUMBER(10,4)
);
comment on table T_31_010_00000
  is '�ɼ����ֱ�׼�����';
comment on column T_31_010_00000.c001
  is '���ֱ�����ɼ�ID,��ӦT_31_008_00000.C001';
comment on column T_31_010_00000.c002
  is '���ֱ��׼����ID,��ӦT_31_002_00000.C001';
comment on column T_31_010_00000.c003
  is '��Ӧ T_31_001_00000.C001';
comment on column T_31_010_00000.c004
  is '��ӦT_31_003_00000.C003,����ID';
comment on column T_31_010_00000.c005
  is '�������ֱ�׼���Ϊ�ı�ʱ��ʾ�ķ���';
alter table T_31_010_00000
  add constraint PK_31_010_0 primary key (C001, C002, C003, C004);