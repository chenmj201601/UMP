create global temporary table T_99_001
(
  C001 VARCHAR2(5),
  C002 NVARCHAR2(2000)
)
on commit preserve rows;
-- Add comments to the table 
comment on table T_99_001
  is '字符串拆分临时表';
-- Add comments to the columns 
comment on column T_99_001.c001
  is '列名';
comment on column T_99_001.c002
  is '列值';
