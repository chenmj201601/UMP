create table T_11_101_00000
(
  C001 NUMBER(20) not null,
  C002 NUMBER(5) not null,
  C011 NVARCHAR2(128),
  C012 NVARCHAR2(128),
  C013 NVARCHAR2(128),
  C014 NVARCHAR2(256),
  C015 NVARCHAR2(256),
  C016 NVARCHAR2(256),
  C017 NVARCHAR2(512),
  C018 NVARCHAR2(512),
  C019 NVARCHAR2(512),
  C020 NVARCHAR2(1024),
  constraint PK_11_101_00000 primary key (C001, C002)
);
comment on table T_11_101_00000
  is 'ϵͳ��Դ���ԣ��û�����ϯ��ʹ�øñ����ݲ��֡�';
comment on column T_11_101_00000.C001
  is '��Դ��š����ݱ�Ź���������Դ���';
comment on column T_11_101_00000.C002
  is '��Դ�����к� 1-9';
comment on column T_11_101_00000.C011
  is '����N1';
