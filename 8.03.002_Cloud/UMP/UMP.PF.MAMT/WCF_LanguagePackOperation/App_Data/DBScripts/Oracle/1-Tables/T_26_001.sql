create table T_26_001
(
  c001 NUMBER(5) not null,
  c002 NUMBER(5) not null,
  c003 NUMBER(5) not null,
  c004 NUMBER(5)
);
-- Add comments to the table 
comment on table T_26_001
  is '������';
-- Add comments to the columns 
comment on column T_26_001.c001
  is '����ID�ţ�����261000����Ϊֻ�����ݹ���';
comment on column T_26_001.c002
  is '�Ƿ����ñ��ݹ���';
comment on column T_26_001.c003
  is '�⻧ID';
comment on column T_26_001.c004
  is '�˷����Ƿ����Զ���豸��1Ϊ��������ϵͳΨһ';
-- Create/Recreate primary, unique and foreign key constraints 
alter table T_26_001
  add constraint PK_26_001 primary key (C001);
