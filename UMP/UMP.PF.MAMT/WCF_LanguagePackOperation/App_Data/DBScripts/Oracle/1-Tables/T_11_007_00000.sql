
create table T_11_007_00000
(
  C001 NUMBER(20) not null,
  C002 NVARCHAR2(256) not null,
  C003 NUMBER(5) not null,
  C004 VARCHAR2(1024) not null,
  C005 VARCHAR2(1024) not null,
  C006 VARCHAR2(1024),
  C007 CHAR(1) not null,
  C008 CHAR(1) not null,
  C009 NUMBER(20) not null,
  C010 DATE not null,
  C011 NVARCHAR2(1024),
  constraint PK_11_007_00000 primary key (C001)
);
comment on table T_11_007_00000
  is '�򣨹����飩��Ϣ';
comment on column T_11_007_00000.C001
  is '��ϵͳ���';
comment on column T_11_007_00000.C002
  is '������';
comment on column T_11_007_00000.C003
  is '������';
comment on column T_11_007_00000.C004
  is 'ӵ���������Ȩ���û�����������';
comment on column T_11_007_00000.C005
  is '��Ӧ�û������롣��������';
comment on column T_11_007_00000.C006
  is '�����Ŀ¼����������';
comment on column T_11_007_00000.C007
  is '�Ƿ��ǻ�ġ�1�����0�����õ�';
comment on column T_11_007_00000.C008
  is '�Ѿ�ɾ����1��ɾ����0��δɾ��';
comment on column T_11_007_00000.C009
  is '������';
comment on column T_11_007_00000.C010
  is '����ʱ��UTC';
comment on column T_11_007_00000.C011
  is '��ע������';
create unique index IDX_11_007_C002_00000 on T_11_007_00000 (C002);
