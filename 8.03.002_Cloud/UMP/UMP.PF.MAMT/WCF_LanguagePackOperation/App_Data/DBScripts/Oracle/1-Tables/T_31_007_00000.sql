
create table T_31_007_00000
(
  c001 NUMBER(5) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(5) not null,
  c005 NUMBER(10,4) not null,
  c006 NUMBER(10,4) not null,
  c007 CHAR(1) not null,
  c008 NUMBER(5) not null,
  c009 CHAR(1),
  c010 NUMBER(5) not null,
  c011 NUMBER(10,4),
  constraint PK_31_007_0 primary key (C001)
);
comment on table T_31_007_00000
  is '�������';
comment on column T_31_007_00000.c001
  is 'һ�����ֱ�׼�ж��������ʱ��������������ţ�Ĭ�ϴ�0����';
comment on column T_31_007_00000.c002
  is '���ֱ������ID,��ӦT_31_002_00000.c001';
comment on column T_31_007_00000.c003
  is '���ֱ�ID,��ӦT_31_001_00000.c001';
comment on column T_31_007_00000.c004
  is '1����=,2����>=,3����>,4����<=,5����< 6 between
';
comment on column T_31_007_00000.c005
  is '����÷ֵ�ֵ';
comment on column T_31_007_00000.c006
  is '��JugeTypeΪbetweenʱ���Ż����ʵ����ֵ';
comment on column T_31_007_00000.c007
  is '��������ֱ�׼�з��Ƶ��������Է���������ֵ;1��ʾ���ʵ�ʵ÷֣�2��Է��Ƶ÷�';
comment on column T_31_007_00000.c008
  is '��Ӧ���ֱ�ID,T_31_001_00000.C001�������ֱ�����ID,T_31_002_00000.C001';
comment on column T_31_007_00000.c009
  is '1�������ֱ�,2�������ֱ�����';
comment on column T_31_007_00000.c010
  is '1����=��2����+,3����-��4����*��5����/,6����N/A';
comment on column T_31_007_00000.c011
  is '������Ķ���ı��ֵ';
