
create table T_31_030_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(100) not null,
  c003 DATE,
  c004 CHAR(1) default 'Y' not null,
  constraint PK_31_030_0 primary key (C001)
);
comment on table T_31_030_00000
  is '�ʼ�Ա�ȼ������ڸ��ʼ���Ա�ӱ�־����Ҫ�����������';
comment on column T_31_030_00000.c001
  is '�ʼ�Ա�ȼ�ID����������';
comment on column T_31_030_00000.c002
  is '�ʼ�Ա�ȼ�����';
comment on column T_31_030_00000.c003
  is '����ʱ��';
comment on column T_31_030_00000.c004
  is 'YΪ���� NΪ����';
