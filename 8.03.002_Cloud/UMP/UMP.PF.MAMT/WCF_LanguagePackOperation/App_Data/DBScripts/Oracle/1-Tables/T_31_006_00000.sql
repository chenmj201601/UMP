
create table T_31_006_00000
(
  c001 NUMBER(5) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(5) not null,
  c004 NUMBER(20),
  constraint PK_31_006_0 primary key (C001, C002, C003)
);
comment on table T_31_006_00000
  is '���ֱ�׼������ת��,�������ֱ�׼�����ѡ����Щ���ֱ�����Ҫ�����������N/A��';
comment on column T_31_006_00000.c001
  is '���ֱ�����ID ��T_31_002_00000.c001';
comment on column T_31_006_00000.c002
  is '���ֱ�ID ��T_31_001_00000.c001';
comment on column T_31_006_00000.c003
  is ' ˳��ID����1��ʼ';
comment on column T_31_006_00000.c004
  is '���ֱ�����ID ��T_31_002_00000.c001';
