create table T_00_001
(
  C000 VARCHAR2(5) not null,
  C001 NUMBER(5) not null,
  C002 NUMBER(5) not null,
  C003 NUMBER(11) not null,
  C004 NUMBER(11) not null,
  constraint PK_00_001 primary key (C000, C001, C002, C003)
);
comment on table T_00_001
  is '系统自动流水号生成表';
comment on column T_00_001.C000
  is '租户编号. 默认值 00000';
comment on column T_00_001.C001
  is '模块编号（11-91）';
comment on column T_00_001.C002
  is '模块内部编号，范围为 100 - 999';
comment on column T_00_001.C003
  is '使用到的时间，精确到小时。值如 yyyymmddhh。如果该值参数为NoTime，则与时间无关，直接返回 C004 + 1';
comment on column T_00_001.C004
  is '当前已经使用的值 1 - 9,999,999';
