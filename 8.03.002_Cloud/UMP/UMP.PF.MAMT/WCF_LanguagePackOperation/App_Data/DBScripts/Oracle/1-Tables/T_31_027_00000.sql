
create table T_31_027_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20),
  c004 NUMBER(5) not null,
  c005 NUMBER(5),
  c006 NUMBER(5) not null
);
comment on table T_31_027_00000
  is '��һ������ʣ���';
comment on column T_31_027_00000.c001
  is '�Զ��������ID,��Ӧ T_31_023_00000.C001';
comment on column T_31_027_00000.c002
  is '��ϯID ,��Ӧ T_11_005_00000_0.C001';
comment on column T_31_027_00000.c003
  is '��Ӧ�ϸ�����ʣ�µļ�¼';
comment on column T_31_027_00000.c004
  is '���';
comment on column T_31_027_00000.c005
  is '�·�';
comment on column T_31_027_00000.c006
  is '1������Ѯ��2������Ѯ 3������Ѯ';
alter table T_31_027_00000
  add constraint PK_T_31_027_0 primary key (C001, C002)

