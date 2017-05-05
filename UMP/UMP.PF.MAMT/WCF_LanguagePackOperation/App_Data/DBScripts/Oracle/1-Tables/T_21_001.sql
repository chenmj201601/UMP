
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
  is '¼����¼�ӿڱ�';
comment on column T_21_001.c001
  is '��¼��ˮ�ţ�1 - 2147483647 (21��)';
comment on column T_21_001.c002
  is '��¼��ˮ�� YYMMDDHH 000000000';
comment on column T_21_001.c003
  is '�⻧���';
comment on column T_21_001.c004
  is '��ʼ¼��ʱ�䣬¼������������ʱ��';
comment on column T_21_001.c005
  is '��ʼ¼��ʱ�䣬¼��������UTCʱ��';
comment on column T_21_001.c006
  is '��ʼ¼��ʱ�䣬YYYYMMDDHHMISS,UTC';
comment on column T_21_001.c007
  is '��ʼ¼��ʱ�䣬TIME STAMP,UTC';
comment on column T_21_001.c008
  is '����¼��ʱ�䣬¼������������ʱ��';
comment on column T_21_001.c009
  is '����¼��ʱ�䣬¼��������UTCʱ��';
comment on column T_21_001.c010
  is '����¼��ʱ�䣬YYYYMMDDHHMISS,UTC';
comment on column T_21_001.c011
  is '����¼��ʱ�䣬TIME STAMP,UTC';
comment on column T_21_001.c012
  is '¼��ʱ�����룩';
comment on column T_21_001.c013
  is '¼��ʱ����hh:mm:ss��';
comment on column T_21_001.c014
  is 'ý�����ͣ��磨1��¼����2��¼����';
comment on column T_21_001.c015
  is 'ý������ʽ';
comment on column T_21_001.c016
  is '��¼����ʱ�䣬TIME STAMP,UTC';
comment on column T_21_001.c017
  is '��¼������ʱ�䣬TIME STAMP,UTC';
comment on column T_21_001.c018
  is 'UTC Offset UCTʱ���뱾��ʱ���ƫ����(����)';
comment on column T_21_001.c019
  is '¼����������';
comment on column T_21_001.c020
  is '¼��������IP';
comment on column T_21_001.c021
  is '¼����չ������ϵͳ���ã����ֵ������д�磺�ֻ�����ϯ�ŵȡ�';
comment on column T_21_001.c022
  is 'վ����';
comment on column T_21_001.c023
  is '�ޱ���';
comment on column T_21_001.c024
  is '���ұ���';
comment on column T_21_001.c025
  is '���ܰ汾�š�';
comment on column T_21_001.c026
  is 'KeyID';
comment on column T_21_001.c027
  is 'PolicyID';
comment on column T_21_001.c028
  is 'Key1b';
comment on column T_21_001.c029
  is 'Key1d';
comment on column T_21_001.c030
  is '����ʱ�䣬TIME STAMP,UTC';
comment on column T_21_001.c031
  is '���ݴ���';
comment on column T_21_001.c032
  is '���һ�α���ʱ�䣬TIME STAMP,UTC';
comment on column T_21_001.c033
  is '�ӱ���ɾ��¼���ļ�';
comment on column T_21_001.c034
  is 'ɾ��ʱ�䣬TIME STAMP,UTC';
comment on column T_21_001.c035
  is '¼���������·��1';
comment on column T_21_001.c036
  is '¼���������·��2';
comment on column T_21_001.c037
  is '��������ţ�VoiceID';
comment on column T_21_001.c038
  is 'ͨ����ţ�ChannelID';
comment on column T_21_001.c039
  is '��ϯ��';
comment on column T_21_001.c040
  is '���к���';
comment on column T_21_001.c041
  is '���к���';
comment on column T_21_001.c042
  is '�ֻ���(VMC������ֵ)';
comment on column T_21_001.c043
  is '����DTMF';
comment on column T_21_001.c044
  is '����DTMF';
comment on column T_21_001.c045
  is '���з���1�����룻0��������';
comment on column T_21_001.c046
  is 'ͨ������(VMC������ChannelName)';
comment on column T_21_001.c047
  is 'CTI��ˮ��';
comment on column T_21_001.c048
  is 'First CTI��ˮ��';
comment on column T_21_001.c049
  is 'Last CTI��ˮ��';
comment on column T_21_001.c050
  is 'ת�ӱ�־��1��ת�룻0��ת��';
comment on column T_21_001.c051
  is 'ת��(Դ);ת��(ȥ��)';
comment on column T_21_001.c052
  is '�����־��1����������룻0����������';
comment on column T_21_001.c053
  is '����Դ';
comment on column T_21_001.c054
  is '��绰���룬�м��� ��Ƕ��� ��,�� �ֿ�';
comment on column T_21_001.c055
  is 'ͨ���е���������������';
comment on column T_21_001.c056
  is 'CRCУ����';
comment on column T_21_001.c057
  is 'Ŀǰֻ��������VoIPCallID���ݡ�';
comment on column T_21_001.c058
  is '��ʵ�ֻ�����';
comment on column T_21_001.c059
  is 'Hold����';
comment on column T_21_001.c060
  is 'Hold��ʱ�����룩';
comment on column T_21_001.c061
  is '����ʱ�����룩';
comment on column T_21_001.c062
  is '��������';
comment on column T_21_001.c063
  is '������ʱ�����룩';
comment on column T_21_001.c064
  is '��ϯ����ʱ�����룩';
comment on column T_21_001.c065
  is '�ͻ�����ʱ�����룩';
comment on column T_21_001.c066
  is 'ת�ӵ���ͨ��ǰ��ת�ӵĴ���(������ת��)';
comment on column T_21_001.c067
  is '�粵����ʱ��';
comment on column T_21_001.c068
  is '���طŴ���';
comment on column T_21_001.c069
  is '�����ش���';
comment on column T_21_001.c070
  is '����¼��д���������������QMģ��ģ�';
comment on column T_21_001.c071
  is '����ע������';
comment on column T_21_001.c072
  is '�������';
comment on column T_21_001.c073
  is '��ע����';
comment on column T_21_001.c074
  is '¼��¼��������ʶ����ϵͳ��';
comment on column T_21_001.c075
  is '¼��¼��������ʶ�����˹���';
comment on column T_21_001.c076
  is 'ʹ�������ֵ�����ܱ���';
comment on column T_21_001.c077
  is '¼����ˮ��';
comment on column T_21_001.c078
  is '�û�����1';
comment on column T_21_001.c079
  is '�û�����2';
comment on column T_21_001.c080
  is '�û�����3';
alter table T_21_001
  add constraint PK_21_001 primary key (C001);