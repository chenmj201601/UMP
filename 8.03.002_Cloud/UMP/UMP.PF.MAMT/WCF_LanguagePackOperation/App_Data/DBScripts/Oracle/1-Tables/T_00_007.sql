create table T_00_007
(
  C001 NUMBER(5) not null,
  C002 NUMBER(5) not null,
  C003 NUMBER(5) not null,
  C004 NUMBER(5) not null,
  C005 NUMBER(5) not null,
  C006 NUMBER(5) not null,
  C007 NVARCHAR2(1024),
  C008 NVARCHAR2(1024),
  C009 NVARCHAR2(1024),
  C010 NVARCHAR2(1024),
  constraint PK_00_007 primary key (C001, C002, C003, C004, C005, C006)
);
comment on table T_00_007
  is '�澯ϵͳʹ�����԰�';
comment on column T_00_007.C001
  is '���Ա��롣�������ģ�2052���������ģ�1028��Ӣ�U.S.����1033';
comment on column T_00_007.C002
  is '�澯Դģ�����ͱ���';
comment on column T_00_007.C003
  is '�澯����';
comment on column T_00_007.C004
  is '�澯��ϢID';
comment on column T_00_007.C005
  is '�澯����ϢID';
comment on column T_00_007.C006
  is '���ͷ�ʽ';
comment on column T_00_007.C007
  is '��Ϣ���ݼ��';
comment on column T_00_007.C008
  is '��Ϣ��������1';
comment on column T_00_007.C009
  is '��Ϣ��������2';
comment on column T_00_007.C010
  is '�������';
