create table T_44_102_00000
(
  c001 NUMBER(20) default 0 not null,
  c002 VARCHAR2(256) not null,
  c003 NVARCHAR2(1024),
  c004 CHAR(1) default '0' not null,
  c005 NUMBER(5) default 0 not null,
  c006 NUMBER(20) default 0 not null,
  c007 NUMBER(20),
  c008 NUMBER(20),
  c009 NUMBER(20),
  c010 NUMBER(20),
  c011 VARCHAR2(32),
  c012 VARCHAR2(32),
  c013 VARCHAR2(32) default '0',
  c014 VARCHAR2(512),
  constraint PK_44_102_00000 primary key (C001)
);