
create table T_31_013_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(5) not null,
  c003 NUMBER(10) not null,
  c004 NUMBER(5) not null,
  c005 NUMBER(10,2) not null,
  c006 NUMBER(10,2) not null,
  c007 CHAR(1) not null,
  c008 NUMBER(20) not null,
  c009 NUMBER(5) not null,
  c010 NVARCHAR2(2000)
);
comment on table T_31_013_00000
  is '�߼��������';
comment on column T_31_013_00000.c001
  is '�������ֱ������ж��������ʱ��������������ţ�Ĭ�ϴ�0����';
comment on column T_31_013_00000.c002
  is '���ֱ������ID,��ӦT_31_002_00000.C001';
comment on column T_31_013_00000.c003
  is '���ֱ�ID,��ӦT_31_001_00000.C001';
comment on column T_31_013_00000.c004
  is '1����=,2����>=,3����>,4����<=,5����<';
comment on column T_31_013_00000.c005
  is '����÷ֵ�ֵ';
comment on column T_31_013_00000.c006
  is '��JugeTypeΪbetweenʱ���Ż����ʵ����ֵ';
comment on column T_31_013_00000.c007
  is '��������ֱ�׼�з��Ƶ��������Է���������ֵ;1��ʾ���ʵ�ʵ÷֣�2��Է��Ƶ÷�';
comment on column T_31_013_00000.c008
  is '��Ӧ���ֱ�ID�������ֱ�����ID';
comment on column T_31_013_00000.c009
  is '1�������ֱ�,2�������ֱ�׼,3��������';
comment on column T_31_013_00000.c010
  is '���㹫ʽ';
alter table T_31_013_00000
  add constraint PK_31_013_0 primary key (C001);