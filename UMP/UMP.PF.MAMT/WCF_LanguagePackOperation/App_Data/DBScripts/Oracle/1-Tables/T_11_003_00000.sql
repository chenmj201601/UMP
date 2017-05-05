
create table T_11_003_00000
(
  C001 NUMBER(5) not null,
  C002 NUMBER(20) not null,
  C003 NUMBER(20) not null,
  C004 NUMBER(5) not null,
  C005 VARCHAR2(32) not null,
  C006 VARCHAR2(1024),
  C007 VARCHAR2(1024),
  C008 VARCHAR2(1024),
  C009 NVARCHAR2(1024) not null,
  C010 NVARCHAR2(1024),
  C011 CHAR(1) not null,
  C012 CHAR(1),
  C013 NVARCHAR2(1024),
  C014 CHAR(4),
  C015 VARCHAR2(1024),
  constraint PK_T_11_003_0 primary key (C001, C002)
);
comment on table T_11_003_00000
  is 'ϵͳ���ܺͲ����б�';
comment on column T_11_003_00000.C001
  is 'ģ���š���Ӧֵ T_11_002.C001';
comment on column T_11_003_00000.C002
  is '���ܱ�Ż������š��ڸ��ֶ��Ͻ���Ψһ����';
comment on column T_11_003_00000.C003
  is '�������';
comment on column T_11_003_00000.C004
  is '�����������';
comment on column T_11_003_00000.C005
  is 'Ĭ��GroupID����¼��������ķ���';
comment on column T_11_003_00000.C006
  is '�����Ϣ';
comment on column T_11_003_00000.C007
  is '�����Ϣ';
comment on column T_11_003_00000.C008
  is '�����Ϣ';
comment on column T_11_003_00000.C009
  is '�򿪶���(Url)';
comment on column T_11_003_00000.C010
  is '�򿪶�����Ҫ���Ĳ���';
comment on column T_11_003_00000.C011
  is '�򿪷�ʽ��N��������; �� ���������ͣ�M��Menu��B��Button';
comment on column T_11_003_00000.C012
  is 'B:��ǰ����ʾ�ָ�����A���ں�����ʾ�ָ�����N����';
comment on column T_11_003_00000.C013
  is '��ʾͼƬ��·��';
comment on column T_11_003_00000.C014
  is '����С���С�����ͼ�꣬�ֱ��� 0��1��ʾ�Ƿ����';
comment on column T_11_003_00000.C015
  is '��ʾ���������ʶ';
alter table T_11_003_00000
  add constraint CH_T_11_003_0 unique (C002)
;
