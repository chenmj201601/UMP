
create table T_11_201_00000
(
  C001 NUMBER(20) not null,
  C002 NUMBER(20) not null,
  C003 NUMBER(20) not null,
  C004 NUMBER(20) not null,
  C005 DATE not null,
  C006 DATE not null,
  constraint PK_11_201_000000 primary key (C001, C003, C004)
);
comment on table T_11_201_00000
  is '�û�����ɫ�������Ƶ���Դ�б�';
comment on column T_11_201_00000.C001
  is '���';
comment on column T_11_201_00000.C002
  is '������ţ���Ӧ���� C001��Ĭ��ֵ 0';
comment on column T_11_201_00000.C003
  is '�û�����ɫ����������';
comment on column T_11_201_00000.C004
  is '�����ơ��������ı��';
comment on column T_11_201_00000.C005
  is '��ʼʱ��';
comment on column T_11_201_00000.C006
  is '����ʱ��';
create index IDX_11_201_C003_000000 on T_11_201_00000 (C003);

