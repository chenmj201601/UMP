
create table T_31_005_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) default 0 not null,
  c003 CHAR(1),
  c004 NVARCHAR2(2000) not null,
  constraint PK_31_005_0 primary key (C001)
);
comment on table T_31_005_00000
  is '��ע��Ϣ�ӱ�';
comment on column T_31_005_00000.c001
  is '������Ϊ�������ȷ��ʱ��˳��ID����1��ʼ';
comment on column T_31_005_00000.c002
  is '��ע��ID��ӦT_31_004_00000.C001';
comment on column T_31_005_00000.c003
  is 'Ϊ���ı����ʱ���Ƿ�ñ�ע���ѡ�� YΪѡ�� NΪ��ѡ��';
comment on column T_31_005_00000.c004
  is '�����ı�';
