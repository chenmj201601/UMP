
create table T_31_032_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(200) not null,
  c003 NUMBER(5) not null,
  c004 NUMBER(5) not null,
  c005 CHAR(1) not null,
  c006 CHAR(1) not null,
  constraint PK_31_032_0 primary key (C001)
);
comment on table T_31_032_00000
  is '��������и���ʱ����¼������������';
comment on column T_31_032_00000.c001
  is '������������������';
comment on column T_31_032_00000.c002
  is '�洢��������,�Զ��Ÿ���';
comment on column T_31_032_00000.c003
  is '1����=,2����>=,3����>,4����<=,5����< 6 between';
comment on column T_31_032_00000.c004
  is '�������������ȼ�';
comment on column T_31_032_00000.c005
  is 'YΪ���� NΪ����';
comment on column T_31_032_00000.c006
  is 'RΪ¼�����������ƣ�LΪ¼��ʱ��������';
