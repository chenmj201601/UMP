create table T_00_002
(
  C000 VARCHAR2(5) not null,
  C001 NUMBER(20) not null,
  C002 NUMBER(5) not null,
  C003 VARCHAR2(128) not null,
  C004 DATE not null,
  C005 NVARCHAR2(1024) not null,
  C006 NVARCHAR2(1024) not null,
  C008 NUMBER(5) not null,
  C009 NUMBER(20) not null,
  constraint PK_00_002 primary key (C000, C001, C003, C004)
);
comment on table T_00_002
  is 'ϵͳ���ݵĹؼ����ݱ����¼��Ϣ';
comment on column T_00_002.C000
  is '�⻧���. Ĭ��ֵ 00000';
comment on column T_00_002.C001
  is '������';
comment on column T_00_002.C002
  is '�������ͣ����û�����ɫ���⻧��';
comment on column T_00_002.C003
  is '�������';
comment on column T_00_002.C004
  is '���ʱ�� UTC';
comment on column T_00_002.C005
  is '���ǰ����';
comment on column T_00_002.C006
  is '���������';
comment on column T_00_002.C008
  is '�����Ƿ���ܣ������ܰ汾��0Ϊ������';
comment on column T_00_002.C009
  is '����ˡ�0��ϵͳ�Զ����';
