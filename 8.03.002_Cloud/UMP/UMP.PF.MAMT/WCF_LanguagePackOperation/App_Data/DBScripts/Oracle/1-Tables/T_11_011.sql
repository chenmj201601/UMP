create table *****
(
  c001 NUMBER(20) not null,
  c002 NUMBER(11) not null,
  c003 NUMBER(11) default 0 not null,
  c004 NUMBER(5) default 0 not null,
  c005 NVARCHAR2(1024) not null,
  c006 NUMBER(5) not null,
  c007 NVARCHAR2(1024),
  c008 NUMBER(5) default 0 not null,
  c009 NUMBER(11) default 0 not null,
  c010 VARCHAR2(1024),
  c011 NVARCHAR2(1024),
  c012 NVARCHAR2(1024),
  c013 VARCHAR2(512),
  c014 NUMBER(20),
  c015 VARCHAR2(512),
  c016 NVARCHAR2(1024),
  constraint PK_***** primary key (C001, C002)
);
-- Add comments to the table 
comment on table *****
  is '�û�������';
-- Add comments to the columns 
comment on column *****.c001
  is '�û���ţ�19λ��';
comment on column *****.c002
  is '�������';
comment on column *****.c003
  is '�����������ţ�Ĭ��ֵ 0�������Ҫ�Ըñ��еĲ������з��飬��ô������д�����ţ�';
comment on column *****.c004
  is '����������ţ�Ĭ��ֵ 0����ĳһ���������˳��';
comment on column *****.c005
  is '����ֵ';
comment on column *****.c006
  is '�������ͣ�1��smallint��2��int��3��bigint��4��number��11��char��12��nchar��13��varchar��14��nvarchar��21��datetime��';
comment on column *****.c007
  is '������Ч�Լ�鷽���������Ǻ����';
comment on column *****.c008
  is '��ʾ��ʽת����Ĭ��ֵ 0��';
comment on column *****.c009
  is '���ݴ�BasicInformation������Դ����Ҫ';
comment on column *****.c010
  is '������ʽ��֤';
comment on column *****.c011
  is '����ֵ��Χ - ��С(����б�ҪУ�� ParamValue �� ����������д)';
comment on column *****.c012
  is '����ֵ��Χ - ���(����б�ҪУ�� ParamValue �� ����������д)';
comment on column *****.c013
  is '����޸�ʱ��';
comment on column *****.c014
  is 'Key Value Change ID';
comment on column *****.c015
  is '��Чʱ�䡣���ܱ���';
comment on column *****.c016
  is '����ֵ,������Чʱ���,���� C005 ��ֵ,���ֶ����';


