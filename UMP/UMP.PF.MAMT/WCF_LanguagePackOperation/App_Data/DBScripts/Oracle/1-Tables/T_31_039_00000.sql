
create table T_31_039_00000
(
  c001 NUMBER(20) not null,
  c002 VARCHAR2(100) not null,
  c003 NUMBER(5) default 100 not null,
  constraint PK_31_039_0 primary key (C001, C002)
);
comment on table T_31_039_00000
  is '��ѯ����Զ�����';
comment on column T_31_039_00000.c001
  is '�û�ID ,��Ӧ T_11_005_00000.C001';
comment on column T_31_039_00000.c002
  is '�����Զ��Ÿ���';
comment on column T_31_039_00000.c003
  is '�洢�еĿ��';

