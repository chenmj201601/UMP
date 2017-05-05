
create table T_31_002_00000
(
  c001 NUMBER(5) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(20),
  c005 NUMBER(5) not null,
  c006 NVARCHAR2(1024),
  c007 NVARCHAR2(1024),
  c008 NUMBER(10,4),
  c009 CHAR(1) default 'N' not null,
  c010 CHAR(1) default 'N' not null,
  c011 CHAR(1) default 'N' not null,
  c012 CHAR(1) default 'N' not null,
  c013 CHAR(1) default 'N' not null,
  c014 CHAR(1) default 'N',
  c015 NUMBER(5) default 1 not null,
  c016 CHAR(1) default 'N' not null,
  c017 CHAR(1) default 'N' not null,
  c018 NUMBER(10,4),
  c019 CHAR(1) default 'N' not null,
  c020 CHAR(1) default 'S' not null,
  c021 CHAR(1),
  c022 NVARCHAR2(1024),
  c023 NUMBER(10,4),
  c024 NUMBER(10,4),
  c025 CHAR(100),
  c026 NUMBER(5),
  c027 NUMBER(10,4),
  c028 NUMBER(10,4),
  c029 NUMBER(10,4),
  c030 NVARCHAR2(1024),
  constraint PK_31_002_0 primary key (C001, C003)
);
comment on table T_31_002_00000
  is '评分表子项表';
comment on column T_31_002_00000.c001
  is '评分表子项ID，1~200间数字，与T_31_001_0.C001一起组成联合主键';
comment on column T_31_002_00000.c002
  is '评分表子项ID,通过调用存储过程生成';
comment on column T_31_002_00000.c003
  is '评分表ID，对应T_31_001_0.C001';
comment on column T_31_002_00000.c004
  is '子项父项ID,对应本身的C001';
comment on column T_31_002_00000.c005
  is '处于同一结点时的排列顺序';
comment on column T_31_002_00000.c006
  is '子项名称';
comment on column T_31_002_00000.c007
  is '子项描述';
comment on column T_31_002_00000.c008
  is '该项总分';
comment on column T_31_002_00000.c009
  is 'Y表示是评分标准 N表示不是评分标准';
comment on column T_31_002_00000.c010
  is 'Y表示不计入总分，N表示计入总分';
comment on column T_31_002_00000.c011
  is 'Y表示是控制项 N表示不是';
comment on column T_31_002_00000.c012
  is 'Y表示是关键项 N表示非关键项，O表一般项';
comment on column T_31_002_00000.c013
  is 'Y表示是允许N/A项 N表示不是';
comment on column T_31_002_00000.c014
  is 'Y表示带跳转项,N表不是跳转项';
comment on column T_31_002_00000.c015
  is '1表示文本风格,2表示下拉风格，3表示多选风格,4表示单选风格，5表示标尺拖拉风格 6开关模式';
comment on column T_31_002_00000.c016
  is '是否将这个的分数平均其子项的评分标准，其子项必须全为评分标准，Y表示平均分配，N表示不是';
comment on column T_31_002_00000.c017
  is 'Y表示是附加分 N表示不是';
comment on column T_31_002_00000.c018
  is '分制';
comment on column T_31_002_00000.c019
  is 'Y表启用分制 ,N表不启用分制';
comment on column T_31_002_00000.c020
  is 'P：百分比 F:纯是非 S:数值';
comment on column T_31_002_00000.c021
  is 'S是简单公式 A高级版公式';
comment on column T_31_002_00000.c022
  is '建议';
comment on column T_31_002_00000.c023
  is '标尺拖拉风格时开始值';
comment on column T_31_002_00000.c024
  is '标尺拖拉风格时结束值';
comment on column T_31_002_00000.c025
  is '标尺刻度，多个值以逗号分隔';
comment on column T_31_002_00000.c026
  is '标尺平均分成几等份';
comment on column T_31_002_00000.c027
  is '为文本框风格时输入的最大值';
comment on column T_31_002_00000.c028
  is '为文本框风格时输入的最小值';
comment on column T_31_002_00000.c029
  is '为文本框风格时默认输入值';
comment on column T_31_002_00000.c030
  is '正则表达式验证文本框内容是否符合规则';

