create table T_00_101
(
  C001 NUMBER(5) not null,
  C002 NUMBER(20) not null,
  C003 NUMBER(5) not null,
  C004 CHAR(1),
  constraint PK_00_101 primary key (C002)
);
comment on table T_00_101
  is '���ӻ�ʵ�����ͼ';
comment on column T_00_101.c001
  is 'ģ����';
comment on column T_00_101.c002
  is 'ʵ��/��ͼ���';
comment on column T_00_101.c003
  is 'ʵ�����';
comment on column T_00_101.c004
  is '�û�ʵ�� = ''1''��ϵͳά��ʵ�� = ''S''';
create unique index IDX_00_101_C0102 on T_00_101 (C001, C002)
