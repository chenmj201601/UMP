
create table T_31_012_00000
(
  c001 NUMBER(20) not null,
  c002 CHAR(1) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(10,4),
  c005 NVARCHAR2(200),
  c006 NVARCHAR2(200),
  c007 NVARCHAR2(200),
  c008 NVARCHAR2(200),
  c009 NUMBER(10,4),
  c010 NUMBER(10,4)
);
comment on table T_31_012_00000
  is '���ֱ���ʽ��';
comment on column T_31_012_00000.c001
  is '��������ID,���ֱ���ʽ������';
comment on column T_31_012_00000.c002
  is 'T���ֱ���ʽ��I���ֱ�������ʽ,C��ע��ʽ';
comment on column T_31_012_00000.c003
  is '���ֱ�ID��T_31_001_00000.C001';
comment on column T_31_012_00000.c004
  is '�����С';
comment on column T_31_012_00000.c005
  is '�����ϸ';
comment on column T_31_012_00000.c006
  is '��������';
comment on column T_31_012_00000.c007
  is 'ǰ��ɫ';
comment on column T_31_012_00000.c008
  is '����ɫ';
comment on column T_31_012_00000.c009
  is '��';
comment on column T_31_012_00000.c010
  is '��';
alter table T_31_012_00000
  add constraint PK_31_012_0 primary key (C001);