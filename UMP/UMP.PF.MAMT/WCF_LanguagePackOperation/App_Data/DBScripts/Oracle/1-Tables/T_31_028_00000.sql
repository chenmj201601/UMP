
create table T_31_028_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(20) not null,
  c005 DATE not null,
  c006 CHAR(1) not null,
  c007 NUMBER(5) not null,
  c008 DATE
);
comment on table T_31_028_00000
  is '���ٲ�ѯ����Ĭ�ϲ�ѯ';
comment on column T_31_028_00000.c001
  is '���ٲ�ѯID';
comment on column T_31_028_00000.c002
  is '�û�ID ,��Ӧ T_11_034_BU.UserID';
comment on column T_31_028_00000.c003
  is '��ѯ����ID,��Ӧ T_31_024_QueryParam.QueryID';
comment on column T_31_028_00000.c004
  is '���øò�ѯ�������ˣ�������Լ��趨�������Ӧ�û��Լ�ID';
comment on column T_31_028_00000.c005
  is '����ʱ��';
comment on column T_31_028_00000.c006
  is 'S:����  O:�����ǹ����߸����趨��ǿ��ʹ�õĲ�ѯ����';
comment on column T_31_028_00000.c007
  is 'Ĭ��Ϊ0���ж���Ļ�+1';
comment on column T_31_028_00000.c008
  is '���һ��ʹ�õ�ʱ��';
alter table T_31_028_00000
  add constraint PK_31_028_0 primary key (C001)

