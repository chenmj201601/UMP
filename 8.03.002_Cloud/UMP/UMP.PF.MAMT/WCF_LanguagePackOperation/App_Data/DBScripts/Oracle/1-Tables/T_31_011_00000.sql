create table T_31_011_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NVARCHAR2(2000) not null,
  c004 NUMBER(5) not null
);
comment on table T_31_011_00000
  is '�ɼ���ע��';
comment on column T_31_011_00000.c001
  is '���ֳɼ���ID,��ӦT_31_006_00000.c001';
comment on column T_31_011_00000.c002
  is '��ע��ID,��ӦT_31_004_00000.c001';
comment on column T_31_011_00000.c003
  is '�ı���ʱ��ע�������';
comment on column T_31_011_00000.c004
  is '��ע��Ϣ�ӱ�ID,��ӦT_31_005_00000.C001(��¼���ı����ı�עʱ����¼��ѡ�е�����)';
alter table T_31_011_00000
  add constraint PK_31_011_0 primary key (C001, C002, C004);


