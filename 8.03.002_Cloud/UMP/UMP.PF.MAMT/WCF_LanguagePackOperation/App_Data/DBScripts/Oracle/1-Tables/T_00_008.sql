create table T_00_008
(
  C001 NUMBER(5) not null,
  C002 NUMBER(5) not null,
  C003 VARCHAR2(1024) not null,
  C004 VARCHAR2(1024) not null,
  C005 VARCHAR2(1024),
  C006 VARCHAR2(1024),
  C007 VARCHAR2(1024),
  C008 CHAR(1) not null,
  C009 NUMBER(5) not null,
  C010 NVARCHAR2(1024),
  C011 NVARCHAR2(1024),
  C012 CHAR(1),
  C013 NVARCHAR2(1024),
  C014 NVARCHAR2(1024),
  C015 CHAR(4) not null,
  constraint PK_00_008 primary key (C001)
);
comment on table T_00_008
  is '系统模块清单表';
comment on column T_00_008.C001
  is '模块编号。顶级模块编号 为 11 ～ 91';
comment on column T_00_008.C002
  is '父级模块。默认值0，目前不使用该字段';
comment on column T_00_008.C003
  is '许可信息1，该模块是否能使用，采用加密数据保存';
comment on column T_00_008.C004
  is '许可信息2，许可期限';
comment on column T_00_008.C005
  is '许可信息';
comment on column T_00_008.C006
  is '许可信息';
comment on column T_00_008.C007
  is '许可信息';
comment on column T_00_008.C008
  is '是否为独立产品。1：是；0：否';
comment on column T_00_008.C009
  is '排列序号';
comment on column T_00_008.C010
  is '显示图片的路径';
comment on column T_00_008.C011
  is '打开对象(Url)';
comment on column T_00_008.C012
  is '打开方式。N：正常打开';
comment on column T_00_008.C013
  is '其他参数';
comment on column T_00_008.C014
  is '模块描述';
comment on column T_00_008.C015
  is '存在小、中、宽、大图标，分别用 0、1表示是否存在';
