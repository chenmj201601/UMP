
create table T_11_901_00000
(
  C001 NUMBER(20) not null,
  C002 NUMBER(20),
  C003 NUMBER(5) not null,
  C004 NUMBER(20) not null,
  C005 NUMBER(20) not null,
  C006 NVARCHAR2(256) not null,
  C007 NVARCHAR2(512) not null,
  C008 DATE not null,
  C009 NVARCHAR2(256) not null,
  C010 NVARCHAR2(256),
  C011 NVARCHAR2(1024),
  C012 NVARCHAR2(1024),
  C013 NVARCHAR2(1024),
  C014 NVARCHAR2(1024),
  C015 NVARCHAR2(1024),
  C016 NVARCHAR2(1024),
  C017 NVARCHAR2(1024),
  C018 NVARCHAR2(1024),
  C019 NVARCHAR2(1024),
  C020 NVARCHAR2(1024),
  constraint PK_11_901_00000 primary key (C001)
);
comment on table T_11_901_00000
  is '�û�������־��';
comment on column T_11_901_00000.C001
  is '������־��ˮ��';
comment on column T_11_901_00000.C002
  is '�ͻ���SessionID';
comment on column T_11_901_00000.C003
  is 'ģ��ID';
comment on column T_11_901_00000.C004
  is '���ܲ������';
comment on column T_11_901_00000.C005
  is '�����û�ID';
comment on column T_11_901_00000.C006
  is '������';
comment on column T_11_901_00000.C007
  is '����IP';
comment on column T_11_901_00000.C008
  is '����ʱ�� UTC';
comment on column T_11_901_00000.C009
  is '�������';
comment on column T_11_901_00000.C010
  is '�������ݶ�Ӧ�����԰�ID';
comment on column T_11_901_00000.C011
  is '�滻����01';
comment on column T_11_901_00000.C012
  is '�滻����02';
comment on column T_11_901_00000.C013
  is '�滻����03';
comment on column T_11_901_00000.C014
  is '�滻����04';
comment on column T_11_901_00000.C015
  is '�滻����05';
comment on column T_11_901_00000.C016
  is '�쳣����01';
comment on column T_11_901_00000.C017
  is '�쳣����02';
comment on column T_11_901_00000.C018
  is '�쳣����03';
comment on column T_11_901_00000.C019
  is '�쳣����04';
comment on column T_11_901_00000.C020
  is '�쳣����05';
create unique index IDX_11_901_C080504_00000 on T_11_901_00000 (C008, C005, C004);
