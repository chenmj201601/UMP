
create table T_31_036_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(1024) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(20) not null,
  constraint PK_31_036_0 primary key (C001));
comment on table T_31_036_00000
  is '�������������з���Ȩ�޵�������ֻ���в鿴����Ȩ�޵����ܿ�������';
comment on column T_31_036_00000.c001
  is '��������';
comment on column T_31_036_00000.c002
  is '¼����ˮ��,��Ӧ ��ӦT_21_000.RecoredReference';
comment on column T_31_036_00000.c003
  is '��������';
comment on column T_31_036_00000.c004
  is '������ID����Ӧ T_11_005_00000.C001';
