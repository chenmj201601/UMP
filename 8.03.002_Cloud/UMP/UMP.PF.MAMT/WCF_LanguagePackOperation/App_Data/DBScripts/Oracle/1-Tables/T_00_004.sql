create table T_00_004
(
  C001 NUMBER(5) not null,
  C002 NUMBER(5) not null,
  C003 NVARCHAR2(1024),
  C004 VARCHAR2(16) not null,
  C005 CHAR(1) not null,
  C006 CHAR(1) not null,
  constraint PK_00_004 primary key (C001)
);
comment on table T_00_004
  is 'ϵͳ֧�ֵ�����';
comment on column T_00_004.c001
  is '���Ա��� 2052���������ģ�1028���������ģ�1033��Ӣ���������';
comment on column T_00_004.c002
  is '����˳��';
comment on column T_00_004.c003
  is '��ʾͼ��';
comment on column T_00_004.c004
  is '��ǰ�汾';
comment on column T_00_004.c005
  is '�û��Լ��޸Ĺ� ��0��δ�޸ģ���1�������޸�';
comment on column T_00_004.c006
  is '�Ƿ��Ѿ�֧��';
