
create table T_11_102_00000
(
  C001 NUMBER(5) not null,
  C002 NUMBER(5) not null,
  C003 NUMBER(5) not null,
  C004 CHAR(1) not null,
  C005 NVARCHAR2(1024),
  C006 NUMBER(5) not null,
  C007 NUMBER(5) not null,
  C008 NUMBER(5),
  C009 NVARCHAR2(1024),
  C010 NUMBER(11) not null,
  C011 VARCHAR2(1024),
  C012 NVARCHAR2(1024),
  C013 NVARCHAR2(1024),
  C014 NVARCHAR2(1024),
  C015 NVARCHAR2(1024),
  C016 DATE not null,
  C017 NUMBER(20),
 constraint PK_11_102_00000 primary key (C001, C002)
);
comment on table T_11_102_00000
  is 'ϵͳ��Դ���ԣ�ÿ�����ԵĶ���';
comment on column T_11_102_00000.C001
  is '��Դ���͡�';
comment on column T_11_102_00000.C002
  is '���Ա�ţ�1-90��';
comment on column T_11_102_00000.C003
  is '�������ͣ�1��smallint��2��int��3��bigint��4��number��11��char��12��nchar��13��varchar��14��nvarchar��21��datetime��';
comment on column T_11_102_00000.C004
  is '�Ƿ�Ϊ����';
comment on column T_11_102_00000.C005
  is '������Ч�Լ�鷽���������Ǻ����';
comment on column T_11_102_00000.C006
  is '��ʾ��ʽת����Ĭ��ֵ 0�� UMPϵͳ�в�ʹ�ã�DBToolʹ�ã�';
comment on column T_11_102_00000.C007
  is '������������';
comment on column T_11_102_00000.C008
  is '�����������';
comment on column T_11_102_00000.C009
  is 'Ĭ��ֵ';
comment on column T_11_102_00000.C010
  is '���ݴ�BasicInformation������Դ����Ҫ';
comment on column T_11_102_00000.C011
  is '������ʽ��֤';
comment on column T_11_102_00000.C012
  is '����ֵ��Χ - ��С(����б�ҪУ�� ParamValue �� ����������д)';
comment on column T_11_102_00000.C013
  is '����ֵ��Χ - ���(����б�ҪУ�� ParamValue �� ����������д)';
comment on column T_11_102_00000.C014
  is '��Чʱ�䡣���ܱ���';
comment on column T_11_102_00000.C015
  is '����ֵ,������Чʱ���,���� C006 ��ֵ,���ֶ����';
comment on column T_11_102_00000.C016
  is '����޸�ʱ��';
comment on column T_11_102_00000.C017
  is '����޸���';
