create table T_00_008
(
  C001 NUMBER(5) not null,
  C002 NUMBER(5) not null,
  C003 VARCHAR2(1024) not null,
  C004 VARCHAR2(1024) not null,
  C005 VARCHAR2(1024),
  C006 VARCHAR2(1024),
  C007 VARCHAR2(1024),
  C008 CHAR(1) not null,
  C009 NUMBER(5) not null,
  C010 NVARCHAR2(1024),
  C011 NVARCHAR2(1024),
  C012 CHAR(1),
  C013 NVARCHAR2(1024),
  C014 NVARCHAR2(1024),
  C015 CHAR(4) not null,
  constraint PK_00_008 primary key (C001)
);
comment on table T_00_008
  is 'ϵͳģ���嵥��';
comment on column T_00_008.C001
  is 'ģ���š�����ģ���� Ϊ 11 �� 91';
comment on column T_00_008.C002
  is '����ģ�顣Ĭ��ֵ0��Ŀǰ��ʹ�ø��ֶ�';
comment on column T_00_008.C003
  is '�����Ϣ1����ģ���Ƿ���ʹ�ã����ü������ݱ���';
comment on column T_00_008.C004
  is '�����Ϣ2���������';
comment on column T_00_008.C005
  is '�����Ϣ';
comment on column T_00_008.C006
  is '�����Ϣ';
comment on column T_00_008.C007
  is '�����Ϣ';
comment on column T_00_008.C008
  is '�Ƿ�Ϊ������Ʒ��1���ǣ�0����';
comment on column T_00_008.C009
  is '�������';
comment on column T_00_008.C010
  is '��ʾͼƬ��·��';
comment on column T_00_008.C011
  is '�򿪶���(Url)';
comment on column T_00_008.C012
  is '�򿪷�ʽ��N��������';
comment on column T_00_008.C013
  is '��������';
comment on column T_00_008.C014
  is 'ģ������';
comment on column T_00_008.C015
  is '����С���С�����ͼ�꣬�ֱ��� 0��1��ʾ�Ƿ����';
