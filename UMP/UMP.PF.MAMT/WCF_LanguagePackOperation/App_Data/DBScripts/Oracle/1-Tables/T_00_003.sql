create table T_00_003
(
  C001 NUMBER(11) not null,
  C002 NUMBER(5) not null,
  C003 NUMBER(11) not null,
  C004 CHAR(1) not null,
  C005 NUMBER(5) not null,
  C006 VARCHAR2(1024) not null,
  C007 NVARCHAR2(1024),
  constraint PK_00_003 primary key (C001, C002)
);
comment on table T_00_003
  is 'ϵͳ�������ݱ�';
comment on column T_00_003.C001
  is 'ModuleID + 000000';
comment on column T_00_003.C002
  is '�ڸò���е�����';
comment on column T_00_003.C003
  is '������ţ���Ӧ�ñ��C001';
comment on column T_00_003.C004
  is '״̬��''1''����,''0''����';
comment on column T_00_003.C005
  is 'ʵ�������Ƿ�ʹ�ü��ܱ��� 0�����ܱ��棬���� 0 ���ܰ汾';
comment on column T_00_003.C006
  is 'ʵ������';
comment on column T_00_003.C007
  is '��ʾͼ��';

