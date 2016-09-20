create table T_00_901
(
  c001 NUMBER(20) not null,
  c002 NUMBER(11) not null,
  c011 NVARCHAR2(512) not null,
  c012 NVARCHAR2(512),
  c013 NVARCHAR2(512),
  c014 NVARCHAR2(512),
  c015 NVARCHAR2(512),
  constraint PK_00_901 primary key (C001, C002)
);
-- Add comments to the table 
comment on table T_00_901
  is '���й�����ʹ�õĹ���������ʱ��ű�';
-- Add comments to the columns 
comment on column T_00_901.c001
  is '��ʱ��ˮ��';
comment on column T_00_901.c002
  is '��� 1-N';
comment on column T_00_901.c011
  is '��ʱ��ֵ1';
comment on column T_00_901.c012
  is '��ʱ��ֵ2';
comment on column T_00_901.c013
  is '��ʱ��ֵ3';
comment on column T_00_901.c014
  is '��ʱ��ֵ4';
comment on column T_00_901.c015
  is '��ʱ��ֵ5';
