create table T_31_031_00000
(
  c001 NUMBER(5) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(10,4) not null,
  c004 NUMBER(20),
  c005 NUMBER(20),
constraint PK_31_031_0 primary key (C001, C002)
);
comment on table T_31_031_00000
  is '������з���ٷֱ�';
comment on column T_31_031_00000.c001
  is '�ʼ���Ա�ȼ�ID';
comment on column T_31_031_00000.c002
  is '��ѯ����ID';
comment on column T_31_031_00000.c003
  is '�ٷֱ�ֵ(��������ۼ� <=100)
';
comment on column T_31_031_00000.c004
  is '�Զ��������ID';
comment on column T_31_031_00000.c005
  is '������и������� ,Ĭ��Ϊ-1';
