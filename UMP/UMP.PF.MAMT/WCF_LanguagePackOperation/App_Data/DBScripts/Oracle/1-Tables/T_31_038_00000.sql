
create table T_31_038_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) not null,
  c004 DATE not null,
  c005 NUMBER(10) not null,
  c006 NUMBER(5) default 1,
  constraint PK_31_038_0 primary key (C001)
);
comment on table T_31_038_00000
  is '��ѯ��ʷ�����б�';
comment on column T_31_038_00000.c001
  is '��������,������ʷID';
comment on column T_31_038_00000.c002
  is '�����Զ��Ÿ���';
comment on column T_31_038_00000.c003
  is '�洢�еĿ��';
comment on column T_31_038_00000.c004
  is '��������';
comment on column T_31_038_00000.c005
  is '����ʱ��';
comment on column T_31_038_00000.c006
  is '1Ϊ��ѯ����;2 CQC����';
