create table T_31_034_00000
(
  c001 NUMBER(20) not null,
  c002 CHAR(1) default 1 not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(20) not null,
  c005 NUMBER(10,4) not null,
  c006 DATE,
  c007 NUMBER(5),
  c008 NUMBER(5),
  constraint PK_31_034_0 primary key (C001)
);
comment on table T_31_034_00000
  is 'ʱ��������ʷ��';
comment on column T_31_034_00000.c001
  is '��������';
comment on column T_31_034_00000.c002
  is 'T��������������ͳ�ƣ�S�����������ͳ��';
comment on column T_31_034_00000.c003
  is '�������š�T_11_008_00000.c001';
comment on column T_31_034_00000.c004
  is '�÷�������ʱ����ϯ��������T_11_006_00000.c001';
comment on column T_31_034_00000.c005
  is 'ʱ�����Ƶ�ֵ';
comment on column T_31_034_00000.c006
  is '���һ��ͳ�Ƶ�ֵ';
comment on column T_31_034_00000.c007
  is 'ͳ�������������';
comment on column T_31_034_00000.c008
  is 'ͳ�����������·�';
