
create table T_31_022_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 CHAR(1),
  c004 NUMBER(20),
  c005 DATE not null,
  c006 NUMBER(5),
  c007 NUMBER(20)
);
comment on table T_31_022_00000
  is '�������¼����ˮ��������ı�';
comment on column T_31_022_00000.c001
  is '����� ��Ӧ T_31_020_00000.C001';
comment on column T_31_022_00000.c002
  is '¼����ˮ��,��Ӧ ��ӦT_21_000.RecoredReference';
comment on column T_31_022_00000.c003
  is 'Y��������������Ϊ����ʱ��, Nû������';
comment on column T_31_022_00000.c004
  is '������,��Ӧ T_11_005_00000.C001';
comment on column T_31_022_00000.c005
  is '����ʱ��';
comment on column T_31_022_00000.c006
  is '1������������2�����������ƶ������� 3�Ƽ�¼��';
comment on column T_31_022_00000.c007
  is '�����¼��������������������ģ�Ϊ��������ID';
alter table T_31_022_00000
  add constraint PK_T_31_022_0 primary key (C001, C002);
