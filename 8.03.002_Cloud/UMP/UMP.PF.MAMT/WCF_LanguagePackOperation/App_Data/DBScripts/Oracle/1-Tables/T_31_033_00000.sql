
create table T_31_033_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 CHAR(1) not null,
  c004 NUMBER(5) not null,
  constraint PK_31_033_0 primary key (C001, C002)
);
comment on table T_31_033_00000
  is '���������ʱ���´η���ʱ�����';
comment on column T_31_033_00000.c001
  is '¼����ˮ��,��Ӧ ��ӦT_21_000.RecoredReference';
comment on column T_31_033_00000.c002
  is '��ѯ����ID';
comment on column T_31_033_00000.c003
  is '�Ƿ�QM�ȼ�����';
comment on column T_31_033_00000.c004
  is '�ʼ���Ա�ȼ�ID';
