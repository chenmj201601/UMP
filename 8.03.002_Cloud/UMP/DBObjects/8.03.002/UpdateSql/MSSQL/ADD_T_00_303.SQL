
-- Create table
create table T_00_303
(
  c001 VARCHAR(1024) not null,
  c002 SMALLINT default 0 not null,
  c003 VARCHAR(1024) not null,
  c004 NVARCHAR(1024),
  c005 SMALLINT default 0 not null,
  c006 SMALLINT default 0 not null,
  c007 SMALLINT default 0 not null,
  c008 NVARCHAR(1024),
  c011 VARCHAR(1024),
  c012 VARCHAR(1024),
  c013 VARCHAR(1024),
  c014 SMALLINT default 0,
  c015 SMALLINT default 0,
  c016 SMALLINT default 0,
  c017 VARCHAR(1024),
  c018 VARCHAR(1024),
  c019 VARCHAR(1024),
  c020 SMALLINT default 0,
  c101 CHAR(1),
  c102 CHAR(1),
  c103 NVARCHAR(32),
  c104 NVARCHAR(32),
  c105 NVARCHAR(32),
  c106 NVARCHAR(1024),
  c107 NVARCHAR(1024),
  c108 NVARCHAR(1024),
  c109 NVARCHAR(1024),
  c110 NVARCHAR(1024),
  constraint PK_00_303 primary key (C001, C002, C003)
);