﻿create table T_00_302
(
  c001 VARCHAR2(1024) not null,
  c002 NUMBER(5) default 0 not null,
  c003 NUMBER(5) default 0 not null,
  c004 VARCHAR2(1024),
  c005 NUMBER(20) default 0 not null,
  c006 NUMBER(5) default 0 not null,
  c007 VARCHAR2(1024),
  c008 VARCHAR2(1024),
  c009 VARCHAR2(1024),
  c010 VARCHAR2(1024),
  c101 CHAR(1),
  c102 CHAR(1),
  c103 NVARCHAR2(32),
  c104 NVARCHAR2(32),
  c105 NVARCHAR2(32),
  c106 NVARCHAR2(1024),
  c107 NVARCHAR2(1024),
  c108 NVARCHAR2(1024),
  c109 NVARCHAR2(1024),
  c110 NVARCHAR2(1024),  
  CONSTRAINT PK_00_302 PRIMARY KEY(c001)
);