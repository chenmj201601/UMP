create table T_00_102
(
  C001 NUMBER(20) not null,
  C002 VARCHAR2(512) not null,
  C003 NUMBER(5) not null,
  C004 NUMBER(5) not null,
  C005 NUMBER(5) not null,
  C006 CHAR(1) not null,
  C007 NUMBER(11) not null,
  C008 CHAR(1) not null,
  C009 NVARCHAR2(256) not null,
  C010 NUMBER(11) not null,
  C011 CHAR(1) not null,
  C012 NUMBER(5) not null,
  C013 VARCHAR2(256),
  C014 NVARCHAR2(1024),
  C015 NVARCHAR2(1024),
  C016 NVARCHAR2(1024),
  C017 NVARCHAR2(1024),
  constraint PK_00_102 primary key (C001, C002)
);
comment on table T_00_102
  is '���ӻ�ʵ�����ͼ�����Զ���';
comment on column T_00_102.C001
  is 'ʵ��/��ͼ���';
comment on column T_00_102.C002
  is '���������ֶ����������ܱ���';
comment on column T_00_102.C003
  is '��������';
comment on column T_00_102.C004
  is '���ݳ���';
comment on column T_00_102.C005
  is 'С��λ��';
comment on column T_00_102.C006
  is '�Ƿ����Ϊ��';
comment on column T_00_102.C007
  is '��ʾ���';
comment on column T_00_102.C008
  is '���뷽ʽ';
comment on column T_00_102.C009
  is '��ʾ��ʽ���� yyyy-MM-dd HH:mm:ss | ###.##0.00 �� style������(Ĭ�ϵ�)';
comment on column T_00_102.C010
  is '�ӻ������ݱ��ж�ȡ����';
comment on column T_00_102.C011
  is '�Ƿ���ʾ';
comment on column T_00_102.C012
  is '��ʾ˳��';
comment on column T_00_102.C013
  is 'SourceTable';
comment on column T_00_102.C014
  is 'SourceColumn';
comment on column T_00_102.C015
  is 'DisplayColumn';
comment on column T_00_102.C016
  is 'WhereCondition';
comment on column T_00_102.C017
  is '˵��������';

