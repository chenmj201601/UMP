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
  is '运行过程中使用的过渡数据临时存放表';
-- Add comments to the columns 
comment on column T_00_901.c001
  is '临时流水号';
comment on column T_00_901.c002
  is '序号 1-N';
comment on column T_00_901.c011
  is '临时数值1';
comment on column T_00_901.c012
  is '临时数值2';
comment on column T_00_901.c013
  is '临时数值3';
comment on column T_00_901.c014
  is '临时数值4';
comment on column T_00_901.c015
  is '临时数值5';
