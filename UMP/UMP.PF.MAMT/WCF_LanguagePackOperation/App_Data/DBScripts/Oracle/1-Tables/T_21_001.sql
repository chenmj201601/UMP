
create table T_21_001
(
  c001 NUMBER(11) not null,
  c002 NUMBER(20) not null,
  c003 VARCHAR2(5) not null,
  c004 DATE not null,
  c005 DATE not null,
  c006 NUMBER(20) default 19770101000000 not null,
  c007 NUMBER(20) default 0 not null,
  c008 DATE not null,
  c009 DATE not null,
  c010 NUMBER(20) default 19770101000000 not null,
  c011 NUMBER(20) default 0 not null,
  c012 NUMBER(5) default 0 not null,
  c013 VARCHAR2(10) default '00:00:00' not null,
  c014 NUMBER(5) default 1 not null,
  c015 VARCHAR2(64),
  c016 NUMBER(20) default 0 not null,
  c017 NUMBER(20) default 0 not null,
  c018 NUMBER(5) default 0 not null,
  c019 NVARCHAR2(128),
  c020 VARCHAR2(64) default '127.0.0.1' not null,
  c021 NVARCHAR2(128) default 'AgentUMP' not null,
  c022 NUMBER(5) default 0 not null,
  c023 VARCHAR2(5),
  c024 VARCHAR2(32),
  c025 CHAR(1) default 'E' not null,
  c026 NUMBER(20),
  c027 NUMBER(20),
  c028 VARCHAR2(1024),
  c029 VARCHAR2(1024),
  c030 NUMBER(20),
  c031 NUMBER(5) default 0 not null,
  c032 NUMBER(20),
  c033 CHAR(1) default 'N' not null,
  c034 NUMBER(20),
  c035 NVARCHAR2(512) not null,
  c036 NVARCHAR2(512),
  c037 NUMBER(5) not null,
  c038 NUMBER(5) not null,
  c039 NVARCHAR2(128),
  c040 NVARCHAR2(128),
  c041 NVARCHAR2(128),
  c042 NVARCHAR2(32) not null,
  c043 NVARCHAR2(128),
  c044 NVARCHAR2(128),
  c045 CHAR(1) not null,
  c046 NVARCHAR2(128),
  c047 VARCHAR2(1024),
  c048 VARCHAR2(128),
  c049 VARCHAR2(128),
  c050 CHAR(1),
  c051 NVARCHAR2(128),
  c052 CHAR(1),
  c053 NVARCHAR2(128),
  c054 NVARCHAR2(1024),
  c055 NVARCHAR2(1024),
  c056 NUMBER(20),
  c057 NVARCHAR2(128),
  c058 NVARCHAR2(128),
  c059 NUMBER(5),
  c060 NUMBER(5),
  c061 NUMBER(5),
  c062 NUMBER(5),
  c063 NUMBER(5),
  c064 NUMBER(5),
  c065 NUMBER(5),
  c066 NUMBER(5) default 0,
  c067 NUMBER(5) default 0,
  c068 NUMBER(5) default 0,
  c069 NUMBER(5) default 0,
  c070 NUMBER(5) default 0,
  c071 NUMBER(5) default 0,
  c072 NVARCHAR2(1024),
  c073 NVARCHAR2(1024),
  c074 NVARCHAR2(128),
  c075 NVARCHAR2(128),
  c076 VARCHAR2(1024),
  c077 VARCHAR2(64) not null,
  c078 VARCHAR2(128),
  c079 VARCHAR2(128),
  c080 VARCHAR2(512)
);
comment on table T_21_001
  is '录音记录接口表';
comment on column T_21_001.c001
  is '记录流水号，1 - 2147483647 (21亿)';
comment on column T_21_001.c002
  is '记录流水号 YYMMDDHH 000000000';
comment on column T_21_001.c003
  is '租户编号';
comment on column T_21_001.c004
  is '开始录音时间，录音服务器本地时间';
comment on column T_21_001.c005
  is '开始录音时间，录音服务器UTC时间';
comment on column T_21_001.c006
  is '开始录音时间，YYYYMMDDHHMISS,UTC';
comment on column T_21_001.c007
  is '开始录音时间，TIME STAMP,UTC';
comment on column T_21_001.c008
  is '结束录音时间，录音服务器本地时间';
comment on column T_21_001.c009
  is '结束录音时间，录音服务器UTC时间';
comment on column T_21_001.c010
  is '结束录音时间，YYYYMMDDHHMISS,UTC';
comment on column T_21_001.c011
  is '结束录音时间，TIME STAMP,UTC';
comment on column T_21_001.c012
  is '录音时长（秒）';
comment on column T_21_001.c013
  is '录音时长（hh:mm:ss）';
comment on column T_21_001.c014
  is '媒体类型，如（1：录音；2：录屏）';
comment on column T_21_001.c015
  is '媒体编码格式';
comment on column T_21_001.c016
  is '记录插入时间，TIME STAMP,UTC';
comment on column T_21_001.c017
  is '记录最后更新时间，TIME STAMP,UTC';
comment on column T_21_001.c018
  is 'UTC Offset UCT时间与本地时间的偏移量(分钟)';
comment on column T_21_001.c019
  is '录音服务器名';
comment on column T_21_001.c020
  is '录音服务器IP';
comment on column T_21_001.c021
  is '录制扩展。根据系统设置，这个值可以填写如：分机、座席号等。';
comment on column T_21_001.c022
  is '站点编号';
comment on column T_21_001.c023
  is '洲编码';
comment on column T_21_001.c024
  is '国家编码';
comment on column T_21_001.c025
  is '加密版本号。';
comment on column T_21_001.c026
  is 'KeyID';
comment on column T_21_001.c027
  is 'PolicyID';
comment on column T_21_001.c028
  is 'Key1b';
comment on column T_21_001.c029
  is 'Key1d';
comment on column T_21_001.c030
  is '加密时间，TIME STAMP,UTC';
comment on column T_21_001.c031
  is '备份次数';
comment on column T_21_001.c032
  is '最后一次备份时间，TIME STAMP,UTC';
comment on column T_21_001.c033
  is '从本机删除录音文件';
comment on column T_21_001.c034
  is '删除时间，TIME STAMP,UTC';
comment on column T_21_001.c035
  is '录音本机存放路径1';
comment on column T_21_001.c036
  is '录音本机存放路径2';
comment on column T_21_001.c037
  is '服务器编号，VoiceID';
comment on column T_21_001.c038
  is '通道编号，ChannelID';
comment on column T_21_001.c039
  is '座席号';
comment on column T_21_001.c040
  is '主叫号码';
comment on column T_21_001.c041
  is '被叫号码';
comment on column T_21_001.c042
  is '分机号(VMC中配置值)';
comment on column T_21_001.c043
  is '主叫DTMF';
comment on column T_21_001.c044
  is '被叫DTMF';
comment on column T_21_001.c045
  is '呼叫方向（1：呼入；0：呼出）';
comment on column T_21_001.c046
  is '通道名称(VMC中配置ChannelName)';
comment on column T_21_001.c047
  is 'CTI流水号';
comment on column T_21_001.c048
  is 'First CTI流水号';
comment on column T_21_001.c049
  is 'Last CTI流水号';
comment on column T_21_001.c050
  is '转接标志。1、转入；0、转出';
comment on column T_21_001.c051
  is '转接(源);转接(去向)';
comment on column T_21_001.c052
  is '会议标志。1、被邀请加入；0、创建会议';
comment on column T_21_001.c053
  is '会议源';
comment on column T_21_001.c054
  is '多电话号码，中间用 半角逗号 “,” 分开';
comment on column T_21_001.c055
  is '通话中第三方以外的与会者';
comment on column T_21_001.c056
  is 'CRC校验码';
comment on column T_21_001.c057
  is '目前只用来保存VoIPCallID数据。';
comment on column T_21_001.c058
  is '真实分机号码';
comment on column T_21_001.c059
  is 'Hold次数';
comment on column T_21_001.c060
  is 'Hold总时长（秒）';
comment on column T_21_001.c061
  is '振铃时长（秒）';
comment on column T_21_001.c062
  is '静音次数';
comment on column T_21_001.c063
  is '静音总时长（秒）';
comment on column T_21_001.c064
  is '座席讲话时长（秒）';
comment on column T_21_001.c065
  is '客户讲话时长（秒）';
comment on column T_21_001.c066
  is '转接到该通道前被转接的次数(含本次转接)';
comment on column T_21_001.c067
  is '辩驳持续时间';
comment on column T_21_001.c068
  is '被回放次数';
comment on column T_21_001.c069
  is '被下载次数';
comment on column T_21_001.c070
  is '整条录音写个评语的总数（含QM模块的）';
comment on column T_21_001.c071
  is '被标注的总数';
comment on column T_21_001.c072
  is '评语标题';
comment on column T_21_001.c073
  is '标注标题';
comment on column T_21_001.c074
  is '录音录屏关联标识串（系统）';
comment on column T_21_001.c075
  is '录音录屏关联标识串（人工）';
comment on column T_21_001.c076
  is '使用密码字典表，加密保存';
comment on column T_21_001.c077
  is '录音流水号';
comment on column T_21_001.c078
  is '用户数据1';
comment on column T_21_001.c079
  is '用户数据2';
comment on column T_21_001.c080
  is '用户数据3';
alter table T_21_001
  add constraint PK_21_001 primary key (C001);