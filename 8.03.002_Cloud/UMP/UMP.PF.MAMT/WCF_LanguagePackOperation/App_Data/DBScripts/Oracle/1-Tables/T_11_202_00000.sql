
create table T_11_202_00000
(
  C001 NUMBER(20) not null,
  C002 NUMBER(20) not null,
  C003 CHAR(1) not null,
  C004 CHAR(1) not null,
  C005 CHAR(1) not null,
  C006 NUMBER(20) not null,
  C007 DATE not null,
  C008 DATE not null,
  C009 DATE not null,
  C010 NVARCHAR2(256),
  C011 NUMBER(20),
  C012 NUMBER(20),
  C013 NUMBER(20),
  C014 NUMBER(20),
  C015 NUMBER(20),
  C016 NUMBER(20),
  C017 NUMBER(20),
  C018 NUMBER(20),
  C019 NUMBER(20),
  C020 NUMBER(20),
constraint PK_11_202_00000 primary key (C001, C002)
);
comment on table T_11_202_00000
  is '��ɫ���û��������빦�ܲ����Ķ�Ӧ��ϵ����Except��ϵ';
comment on column T_11_202_00000.C001
  is '�û�����ɫ����������';
comment on column T_11_202_00000.C002
  is '���ܱ�Ż�������';
comment on column T_11_202_00000.C003
  is '�Ƿ����ʹ�ø�Ȩ�ޣ�1-���ԣ�0-�����ԣ�N-δ����';
comment on column T_11_202_00000.C004
  is '��Ȩ���Ƿ���Ա������·����䣬 1-���ԣ�0-�����ԣ�N-δ����';
comment on column T_11_202_00000.C005
  is 'Ȩ���ջ�ʱ���ѷ�����¼�Ȩ���Ƿ����ջأ�1-���ԣ�0-�����ԣ�N-δ����';
comment on column T_11_202_00000.C006
  is '����޸���';
comment on column T_11_202_00000.C007
  is '����޸�����';
comment on column T_11_202_00000.C008
  is '����ʱ��';
comment on column T_11_202_00000.C009
  is '����ʱ��';
comment on column T_11_202_00000.C010
  is '�����ֶ�';
comment on column T_11_202_00000.C011
  is '��Щ�������Ƶ���Դ��������Ȩ��01';
comment on column T_11_202_00000.C012
  is '��Щ�������Ƶ���Դ��������Ȩ��02';
comment on column T_11_202_00000.C013
  is '��Щ�������Ƶ���Դ��������Ȩ��03';
comment on column T_11_202_00000.C014
  is '��Щ�������Ƶ���Դ��������Ȩ��04';
comment on column T_11_202_00000.C015
  is '��Щ�������Ƶ���Դ��������Ȩ��05';
comment on column T_11_202_00000.C016
  is '��Щ�������Ƶ���Դ��������Ȩ��06';
comment on column T_11_202_00000.C017
  is '��Щ�������Ƶ���Դ��������Ȩ��07';
comment on column T_11_202_00000.C018
  is '��Щ�������Ƶ���Դ��������Ȩ��08';
comment on column T_11_202_00000.C019
  is '��Щ�������Ƶ���Դ��������Ȩ��09';
comment on column T_11_202_00000.C020
  is '��Щ�������Ƶ���Դ��������Ȩ��10';

