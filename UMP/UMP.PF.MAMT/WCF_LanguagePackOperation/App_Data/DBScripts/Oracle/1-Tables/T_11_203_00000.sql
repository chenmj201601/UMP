
create table T_11_203_00000
(
  C001 NUMBER(20) not null,
  C002 NUMBER(20) not null,
  C003 VARCHAR2(512) not null,
  C004 NUMBER(5) not null,
  C005 CHAR(1) not null,
  C006 CHAR(1) not null,
  C007 CHAR(1) not null,
  C008 CHAR(1) not null,
  C009 CHAR(1) not null,
  C010 CHAR(1) not null,
  C011 CHAR(1) not null,
  C012 CHAR(1) not null,
  C013 CHAR(1) not null,
  C014 CHAR(1) not null,
  C015 NVARCHAR2(256),
  constraint PK_11_203_00000 primary key (C001, C002, C003)
);
comment on table T_11_203_00000
  is '�û�����ɫ��ʵ������Ȩ�޶�Ӧ��ϵ��';
comment on column T_11_203_00000.C001
  is '��ɫ���û����';
comment on column T_11_203_00000.C002
  is 'ʵ����';
comment on column T_11_203_00000.C003
  is '���������ֶ����������ܱ���';
comment on column T_11_203_00000.C004
  is '��ʾ˳��';
comment on column T_11_203_00000.C005
  is 'Create';
comment on column T_11_203_00000.C006
  is 'Read';
comment on column T_11_203_00000.C007
  is 'Update';
comment on column T_11_203_00000.C008
  is 'Delete';
comment on column T_11_203_00000.C009
  is 'Append';
comment on column T_11_203_00000.C010
  is 'Share';
comment on column T_11_203_00000.C011
  is 'View Opened';
comment on column T_11_203_00000.C012
  is '����';
comment on column T_11_203_00000.C013
  is '����';
comment on column T_11_203_00000.C014
  is '����';
comment on column T_11_203_00000.C015
  is '��ʾstyle';
