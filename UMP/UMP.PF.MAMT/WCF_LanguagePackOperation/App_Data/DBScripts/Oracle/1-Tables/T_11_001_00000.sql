create table T_11_001_00000
(
  C001 NUMBER(20) not null,
  C002 NUMBER(5) not null,
  C003 NUMBER(11) not null,
  C004 NUMBER(11) not null,
  C005 NUMBER(5) not null,
  C006 NVARCHAR2(1024) not null,
  C007 NUMBER(5) not null,
  C008 NVARCHAR2(1024),
  C009 NUMBER(5) not null,
  C010 CHAR(1) not null,
  C011 NUMBER(11) not null,
  C012 VARCHAR2(1024),
  C013 NVARCHAR2(1024),
  C014 NVARCHAR2(1024),
  C015 CHAR(1) not null,
  C016 VARCHAR2(512),
  C017 VARCHAR2(512),
  C018 VARCHAR2(512),
  C019 NUMBER(20),
  C020 NUMBER(20),
  C021 NVARCHAR2(1024) not null,
  C022 NVARCHAR2(1024),
  constraint PK_11_001_0 primary key (C001, C002, C003)
);
-- Add comments to the table 
comment on table T_11_001_00000
  is 'ȫ�ֲ�����';
-- Add comments to the columns 
comment on column T_11_001_00000.C001
  is '�⻧��ţ���� = 0 ��Ϊ���û��Ĳ�������';
comment on column T_11_001_00000.C002
  is 'ģ����';
comment on column T_11_001_00000.C003
  is '�������';
comment on column T_11_001_00000.C004
  is '�����������ţ�Ĭ��ֵ 0�������Ҫ�Ըñ��еĲ������з��飬��ô������д�����ţ�';
comment on column T_11_001_00000.C005
  is '����������ţ�Ĭ��ֵ 0����ĳһ���������˳��';
comment on column T_11_001_00000.C006
  is '����ֵ';
comment on column T_11_001_00000.C007
  is '�������ͣ�1��smallint��2��int��3��bigint��4��number��11��char��12��nchar��13��varchar��14��nvarchar��21��datetime��';
comment on column T_11_001_00000.C008
  is '������Ч�Լ�鷽���������Ǻ����';
comment on column T_11_001_00000.C009
  is '��ʾ��ʽת����Ĭ��ֵ 0�� UMPϵͳ�в�ʹ�ã�DBToolʹ�ã�';
comment on column T_11_001_00000.C010
  is '�⻧�Ƿ���Լ̳к����øò�����''1'' ���ԣ�''0'' ������';
comment on column T_11_001_00000.C011
  is '���ݴ�BasicInformation������Դ����Ҫ';
comment on column T_11_001_00000.C012
  is '������ʽ��֤';
comment on column T_11_001_00000.C013
  is '����ֵ��Χ - ��С(����б�ҪУ�� ParamValue �� ����������д)';
comment on column T_11_001_00000.C014
  is '����ֵ��Χ - ���(����б�ҪУ�� ParamValue �� ����������д)';
comment on column T_11_001_00000.C015
  is '�ò���ֵ�ı���⻧�����Ƿ�������ֵ��ͬ��Ҳ����һ��ı� ''1'' �ı䣬''0'' ���ı�';
comment on column T_11_001_00000.C016
  is '�ò���ֵ�޸ĺ󣬿���ͬ������Щģ�飬ֻ��� ModuleID = 11 ��Ч';
comment on column T_11_001_00000.C017
  is '�ò���ֵ�޸ĺ󣬿���ͬ������Щģ�飬ֻ��� ModuleID != 11 ��Ч';
comment on column T_11_001_00000.C018
  is '����޸�ʱ��';
comment on column T_11_001_00000.C019
  is '����޸���';
comment on column T_11_001_00000.C020
  is 'Key Value Change ID';
comment on column T_11_001_00000.C021
  is '��Чʱ�䡣���ܱ���';
comment on column T_11_001_00000.C022
  is '����ֵ,������Чʱ���,���� C006 ��ֵ,���ֶ����';
