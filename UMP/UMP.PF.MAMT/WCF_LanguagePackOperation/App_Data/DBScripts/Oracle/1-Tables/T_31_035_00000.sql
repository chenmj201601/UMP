
create table T_31_035_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) default 1 not null,
  c004 NUMBER(20) default 1 not null,
  c005 CHAR(1) not null,
  c006 NUMBER(10,4) not null,
  c007 NUMBER(5),
  c008 CHAR(1),
  constraint PK_31_035_0 primary key (C001)
  );
comment on table T_31_035_00000
  is 'ʱ���������ñ������Ű�������ͳ��';
comment on column T_31_035_00000.c001
  is '����ʱ������ID,��������';
comment on column T_31_035_00000.c002
  is '��������ID';
comment on column T_31_035_00000.c003
  is '�������š�T_11_008_00000.C001';
comment on column T_31_035_00000.c004
  is '�÷�������ʱ����ϯ��������T_11_006_00000.C001';
comment on column T_31_035_00000.c005
  is 'T��������������ͳ�ƣ�S�����������ͳ��';
comment on column T_31_035_00000.c006
  is 'Ԥ��ֵ';
comment on column T_31_035_00000.c007
  is '1����=,2����>=,3����>,4����<=,5����< 6 between';
comment on column T_31_035_00000.c008
  is 'Y���á�N����';
