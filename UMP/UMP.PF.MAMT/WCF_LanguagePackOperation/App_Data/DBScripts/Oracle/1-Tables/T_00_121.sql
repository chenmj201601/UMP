create table T_00_121
(
  C001 NUMBER(20) not null,
  C002 NVARCHAR2(512) not null,
  C011 NVARCHAR2(512) not null,
  C012 NVARCHAR2(512) not null,
  C021 NVARCHAR2(512) not null,
  C022 NVARCHAR2(512) not null,
  constraint PK_00_121 primary key (C001)
);
-- Add comments to the table 
comment on table T_00_121
  is '�⻧�б�';
-- Add comments to the columns 
comment on column T_00_121.C001
  is '�⻧���루100��ͷ��';
comment on column T_00_121.C002
  is '�⻧ȫ��';
comment on column T_00_121.C011
  is '��ʼ����ʱ��';
comment on column T_00_121.C012
  is '��������ʱ��';
comment on column T_00_121.C021
  is '�⻧Ψһ����(5λ������ʹ��)';
comment on column T_00_121.C022
  is '�⻧��¼Ψһ��ʶ��';
