
create table T_31_024_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(128),
  c003 NVARCHAR2(1024),
  c004 NUMBER(5),
  c005 CHAR(1) default 'Y' not null,
  c006 CHAR(1),
  c007 NUMBER(20),
  c008 NUMBER(10,4),
  c009 VARCHAR2(2000),
  c010 VARCHAR2(2000),
  c011 VARCHAR2(2000),
  c012 VARCHAR2(2000),
  c013 VARCHAR2(2000),
  c014 VARCHAR2(2000),
  c015 VARCHAR2(2000),
  c016 VARCHAR2(2000),
  c017 VARCHAR2(2000),
  c018 VARCHAR2(2000),
  c019 VARCHAR2(2000),
  c020 VARCHAR2(2000),
  c021 CHAR(1),
  c022 CHAR(1),
  c023 NUMBER(38),
  c024 DATE,
  c025 DATE,
  c026 CHAR(1),
  c027 VARCHAR2(1024),
  c028 CHAR(1),
  c029 VARCHAR2(2000),
  c030 CHAR(1),
  c031 VARCHAR2(2000),
  c032 CHAR(1),
  c033 VARCHAR2(2000),
  c034 CHAR(1),
  c035 CHAR(1),
  c036 CHAR(1),
  c037 CHAR(1),
  c038 CHAR(1),
  c039 NVARCHAR2(1),
  c040 NVARCHAR2(16),
  c041 NVARCHAR2(32),
  c042 NVARCHAR2(64),
  c043 VARCHAR2(2000),
  c044 CHAR(1),
  c045 VARCHAR2(2000),
  c046 CHAR(1),
  c047 CHAR(1),
  c048 CHAR(1),
  c049 CHAR(1),
  c050 CHAR(1),
  c051 CHAR(1),
  c052 CHAR(1),
  c053 CHAR(1),
  c054 CHAR(1),
  c055 CHAR(1),
  c056 CHAR(1),
  c057 CHAR(1),
  c058 CHAR(1),
  c059 CHAR(1),
  c060 CHAR(1),
  c061 VARCHAR2(2000),
  c062 CHAR(1),
  c063 CHAR(1),
  c064 CHAR(1)
);
comment on table T_31_024_00000
  is '查询参数';
comment on column T_31_024_00000.c001
  is '主键自增';
comment on column T_31_024_00000.c002
  is '任务查询条件名称';
comment on column T_31_024_00000.c003
  is '描述';
comment on column T_31_024_00000.c004
  is '1查询条件配置 2初检自动任务分配条件 3复检自动任务分配条件 4智能任务分配条件';
comment on column T_31_024_00000.c005
  is '启用/禁用  Y/N';
comment on column T_31_024_00000.c006
  is 'A：每个座席每天几条 B：每个座席每天百分比  C:每个座席这段时间取多少条 D:每个座席这段时间百分比  E:这段时间总的多少条 F:这段时间录音总的百分比';
comment on column T_31_024_00000.c007
  is '录音的数量';
comment on column T_31_024_00000.c008
  is '取录音的百分比';
comment on column T_31_024_00000.c009
  is '要查询的用户ID，以逗号隔开';
comment on column T_31_024_00000.c010
  is '要查询的用户ID，以逗号隔开';
comment on column T_31_024_00000.c011
  is '要查询的用户ID，以逗号隔开';
comment on column T_31_024_00000.c012
  is '要查询的用户ID，以逗号隔开';
comment on column T_31_024_00000.c013
  is '要查询的用户ID，以逗号隔开';
comment on column T_31_024_00000.c014
  is '要查询的机构ID，以逗号隔开';
comment on column T_31_024_00000.c015
  is '要查询的机构ID，以逗号隔开';
comment on column T_31_024_00000.c016
  is '要查询的机构ID，以逗号隔开';
comment on column T_31_024_00000.c017
  is '要查询的机构ID，以逗号隔开';
comment on column T_31_024_00000.c018
  is '要查询的机构ID，以逗号隔开';
comment on column T_31_024_00000.c019
  is '要查询的技能组ID，以逗号隔开';
comment on column T_31_024_00000.c020
  is '要查询的技能组ID，以逗号隔开';
comment on column T_31_024_00000.c021
  is '是否启用最近时间 Y则启用 N则启用开始时间~结束时间';
comment on column T_31_024_00000.c022
  is 'Y:年 M:月  D:天  H:小时';
comment on column T_31_024_00000.c023
  is '几年，几月，几天，几小时';
comment on column T_31_024_00000.c024
  is '录音开始时间';
comment on column T_31_024_00000.c025
  is '录音结束时间';
comment on column T_31_024_00000.c026
  is '是否分机号模糊查询';
comment on column T_31_024_00000.c027
  is '分机号拼串，以逗号隔开';
comment on column T_31_024_00000.c028
  is '是否录音流水号模糊查询';
comment on column T_31_024_00000.c029
  is '录音流水号以逗号隔开';
comment on column T_31_024_00000.c030
  is '是否主叫号模糊查询';
comment on column T_31_024_00000.c031
  is '主叫号以逗号隔开';
comment on column T_31_024_00000.c032
  is '是否被叫号模糊查询';
comment on column T_31_024_00000.c033
  is '被叫号以逗号隔开';
comment on column T_31_024_00000.c034
  is '呼叫方向  A 呼入和呼出  O呼出 I呼入';
comment on column T_31_024_00000.c035
  is '是否有录音  A 全部的 Y有录屏 N无录屏';
comment on column T_31_024_00000.c036
  is '是否有情绪分析标志';
comment on column T_31_024_00000.c037
  is '是否带关键字标志';
comment on column T_31_024_00000.c038
  is '是否带录音标签';
comment on column T_31_024_00000.c039
  is '录音保留字段1';
comment on column T_31_024_00000.c040
  is '录音保留字段2';
comment on column T_31_024_00000.c041
  is '录音保留字段3';
comment on column T_31_024_00000.c042
  is '录音保留字段4';
comment on column T_31_024_00000.c043
  is '评分表Id，以逗号分隔';
comment on column T_31_024_00000.c044
  is 'A 全部的 Y被评完分 N未被评过分';
comment on column T_31_024_00000.c045
  is '评分者ID';
comment on column T_31_024_00000.c046
  is '服务水平  Y启用，N禁用';
comment on column T_31_024_00000.c047
  is 'A全部 Y服务态度好  N服务态度差';
comment on column T_31_024_00000.c048
  is '专业水平  Y启用，N禁用';
comment on column T_31_024_00000.c049
  is 'A全部 Y专业水平好  N专业水平 差';
comment on column T_31_024_00000.c050
  is '工作极积度 Y启用，N禁用';
comment on column T_31_024_00000.c051
  is 'A全部 Y工作极积度好  N工作极积度差';
comment on column T_31_024_00000.c052
  is '事后处理效率Y启用，N禁用';
comment on column T_31_024_00000.c053
  is 'A全部 Y 事后处理效率高  N 事后处理效率低';
comment on column T_31_024_00000.c054
  is '录音高峰期 Y启用，N禁用';
comment on column T_31_024_00000.c055
  is 'A全部 H:录音高峰期 C:录音平谷 L:录音低谷';
comment on column T_31_024_00000.c056
  is '重复呼入 Y启用，N禁用';
comment on column T_31_024_00000.c057
  is 'A全部 Y是重复呼入  N不是重复呼入';
comment on column T_31_024_00000.c058
  is '时长趋势  Y启用，N禁用';
comment on column T_31_024_00000.c059
  is 'A全部  Y时长趋势适合,N时长趋势不适合';
comment on column T_31_024_00000.c060
  is '座席等级 Y启用，N禁用';
comment on column T_31_024_00000.c061
  is '座席等级拼串，以逗号隔开';
comment on column T_31_024_00000.c062
  is '异常时长 Y启用，N禁用';
comment on column T_31_024_00000.c063
  is 'A全部  Y是异常时长的,N非异常时长的';
comment on column T_31_024_00000.c064
  is 'A全部 Y新座席 N非新座席';
alter table T_31_024_00000
  add constraint PK_T_31_024_0 primary key (C001);
