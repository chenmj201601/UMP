-- Create table
create table T_51_001
(
  c001 NUMBER(11) not null,
  c002 VARCHAR2(64) not null,
  c003 DATE not null,
  c004 DATE not null,
  c005 VARCHAR2(5) not null,
  c006 NVARCHAR2(128),
  c007 VARCHAR2(64) default '127.0.0.1' not null,
  c008 NVARCHAR2(128),
  c009 NVARCHAR2(32) not null,
  c010 NVARCHAR2(128) default 'AgentUMP' not null,
  c011 NUMBER(5) default 1 not null,
  c012 NUMBER(11) default 1 not null,
  c013 NUMBER(5) default 0 not null,
  c014 NUMBER(11) default 0 not null,
  c101 NUMBER(11) default 0 not null,
  c102 NUMBER(11) default 0 not null,
  c103 NUMBER(11) default 0 not null,
  c104 NUMBER(11) default 0 not null,
  c105 NUMBER(11) default 0 not null,
  c106 NUMBER(11) default 0 not null,
  c107 NUMBER(11) default 0 not null,
  c108 NUMBER(11) default 0 not null,
  c109 NUMBER(11) default 0 not null,
  c110 NUMBER(11) default 0 not null,
  c111 NUMBER(11) default 0 not null,
  c112 NUMBER(11) default 0 not null,
  c113 NUMBER(11) default 0 not null,
  c114 NUMBER(11) default 0 not null,
  c115 NUMBER(11) default 0 not null,
  c116 NUMBER(11) default 0 not null,
  c117 NUMBER(11) default 0 not null,
  c118 NUMBER(11) default 0 not null,
  c119 NUMBER(11) default 0 not null,
  c120 NUMBER(11) default 0 not null,
  c121 NUMBER(11) default 0 not null,
  c122 NUMBER(11) default 0 not null,
  c123 NUMBER(11) default 0 not null,
  c124 NUMBER(11) default 0 not null,
  c125 NUMBER(11) default 0 not null,
  c126 NUMBER(11) default 0 not null,
  c127 NUMBER(11) default 0 not null,
  c128 NUMBER(11) default 0 not null,
  c129 NUMBER(11) default 0 not null,
  c130 NUMBER(11) default 0 not null,
  c131 NUMBER(11) default 0 not null,
  c132 NUMBER(11) default 0 not null,
  c133 NUMBER(11) default 0 not null,
  c134 NUMBER(11) default 0 not null,
  c135 NUMBER(11) default 0 not null,
  c136 NUMBER(11) default 0 not null,
  c137 NUMBER(11) default 0 not null,
  c138 NUMBER(11) default 0 not null,
  c139 NUMBER(11) default 0 not null,
  c140 NUMBER(11) default 0 not null,
  c141 NUMBER(11) default 0 not null,
  c142 NUMBER(11) default 0 not null,
  c143 NUMBER(11) default 0 not null,
  c144 NUMBER(11) default 0 not null,
  c145 NUMBER(11) default 0 not null,
  c146 NUMBER(11) default 0 not null,
  c147 NUMBER(11) default 0 not null,
  c148 NUMBER(11) default 0 not null,
  c149 NUMBER(11) default 0 not null,
  c150 NUMBER(11) default 0 not null,
  c151 NUMBER(11) default 0 not null,
  c152 NUMBER(11) default 0 not null,
  c153 NUMBER(11) default 0 not null,
  c154 NUMBER(11) default 0 not null,
  c155 NUMBER(11) default 0 not null,
  c156 NUMBER(11) default 0 not null,
  c157 NUMBER(11) default 0 not null,
  c158 NUMBER(11) default 0 not null,
  c159 NUMBER(11) default 0 not null,
  c160 NUMBER(11) default 0 not null,
  c171 NUMBER(11) default 0 not null,
  c172 NUMBER(11) default 0 not null,
  c173 NUMBER(11) default 0 not null,
  c174 NUMBER(11) default 0 not null,
  c175 NUMBER(11) default 0 not null,
  c176 NUMBER(11) default 0 not null,
  c177 NUMBER(11) default 0 not null,
  c178 NUMBER(11) default 0 not null,
  c179 NUMBER(11) default 0 not null,
  c180 NUMBER(11) default 0 not null,
  c181 NUMBER(11) default 0 not null,
  c182 NUMBER(11) default 0 not null,
  c183 NUMBER(11) default 0 not null,
  c184 NUMBER(11) default 0 not null,
  c185 NUMBER(11) default 0 not null,
  c186 NUMBER(11) default 0 not null,
  c187 NUMBER(11) default 0 not null,
  c188 NUMBER(11) default 0 not null,
  c189 NUMBER(11) default 0 not null,
  c190 NUMBER(11) default 0 not null,
  c191 NUMBER(11) default 0 not null,
  c192 NUMBER(11) default 0 not null,
  c193 NUMBER(11) default 0 not null,
  c194 NUMBER(11) default 0 not null,
  c195 NUMBER(11) default 0 not null,
  c196 NUMBER(11) default 0 not null,
  c197 NUMBER(11) default 0 not null,
  c198 NUMBER(11) default 0 not null,
  c199 NUMBER(11) default 0 not null,
  c200 NUMBER(11) default 0 not null,
  c201 NUMBER(11) default 0 not null,
  c202 NUMBER(11) default 0 not null,
  c203 NUMBER(11) default 0 not null,
  c204 NUMBER(11) default 0 not null,
  c205 NUMBER(11) default 0 not null,
  c206 NUMBER(11) default 0 not null,
  c207 NUMBER(11) default 0 not null,
  c208 NUMBER(11) default 0 not null,
  c209 NUMBER(11) default 0 not null,
  c210 NUMBER(11) default 0 not null,
  c211 NUMBER(11) default 0 not null,
  c212 NUMBER(11) default 0 not null,
  c213 NUMBER(11) default 0 not null,
  c214 NUMBER(11) default 0 not null,
  c215 NUMBER(11) default 0 not null,
  c216 NUMBER(11) default 0 not null,
  c217 NUMBER(11) default 0 not null,
  c218 NUMBER(11) default 0 not null,
  c219 NUMBER(11) default 0 not null,
  c220 NUMBER(11) default 0 not null,
  c221 NUMBER(11) default 0 not null,
  c222 NUMBER(11) default 0 not null,
  c223 NUMBER(11) default 0 not null,
  c224 NUMBER(11) default 0 not null,
  c225 NUMBER(11) default 0 not null,
  c226 NUMBER(11) default 0 not null,
  c227 NUMBER(11) default 0 not null,
  c228 NUMBER(11) default 0 not null,
  c229 NUMBER(11) default 0 not null,
  c230 NUMBER(11) default 0 not null,
  c231 NUMBER(11) default 0 not null,
  c232 NUMBER(11) default 0 not null,
  c233 NUMBER(11) default 0 not null,
  c234 NUMBER(11) default 0 not null,
  c235 NUMBER(11) default 0 not null,
  c236 NUMBER(11) default 0 not null,
  c237 NUMBER(11) default 0 not null,
  c238 NUMBER(11) default 0 not null,
  c239 NUMBER(11) default 0 not null,
  c240 NUMBER(11) default 0 not null,
  c241 NUMBER(11) default 0 not null,
  c242 NUMBER(11) default 0 not null,
  c243 NUMBER(11) default 0 not null,
  c244 NUMBER(11) default 0 not null,
  c245 NUMBER(11) default 0 not null,
  c246 NUMBER(11) default 0 not null,
  c247 NUMBER(11) default 0 not null,
  c248 NUMBER(11) default 0 not null,
  c249 NUMBER(11) default 0 not null,
  c250 NUMBER(11) default 0 not null,
  c251 NUMBER(11) default 0 not null,
  c252 NUMBER(11) default 0 not null,
  c253 NUMBER(11) default 0 not null,
  c254 NUMBER(11) default 0 not null,
  c255 NUMBER(11) default 0 not null,
  c256 NUMBER(11) default 0 not null,
  c257 NUMBER(11) default 0 not null,
  c258 NUMBER(11) default 0 not null,
  c259 NUMBER(11) default 0 not null,
  c260 NUMBER(11) default 0 not null,
  c271 NUMBER(11) default 0 not null,
  c272 NUMBER(11) default 0 not null,
  c273 NUMBER(11) default 0 not null,
  c274 NUMBER(11) default 0 not null,
  c275 NUMBER(11) default 0 not null,
  c276 NUMBER(11) default 0 not null,
  c277 NUMBER(11) default 0 not null,
  c278 NUMBER(11) default 0 not null,
  c279 NUMBER(11) default 0 not null,
  c280 NUMBER(11) default 0 not null,
  c281 NUMBER(11) default 0 not null,
  c282 NUMBER(11) default 0 not null,
  c283 NUMBER(11) default 0 not null,
  c284 NUMBER(11) default 0 not null,
  c285 NUMBER(11) default 0 not null,
  c286 NUMBER(11) default 0 not null,
  c287 NUMBER(11) default 0 not null,
  c288 NUMBER(11) default 0 not null,
  c289 NUMBER(11) default 0 not null,
  c290 NUMBER(11) default 0 not null,
  c291 NUMBER(11) default 0 not null,
  c292 NUMBER(11) default 0 not null,
  c293 NUMBER(11) default 0 not null,
  c294 NUMBER(11) default 0 not null,
  c295 NUMBER(11) default 0 not null,
  c296 NUMBER(11) default 0 not null,
  c297 NUMBER(11) default 0 not null,
  c298 NUMBER(11) default 0 not null,
  c299 NUMBER(11) default 0 not null,
  c300 NUMBER(11) default 0 not null,
  constraint PK_51_001 primary key (C001, C011, C013)
);
-- Add comments to the table 
comment on table T_51_001
  is '�����������';
-- Add comments to the columns 
comment on column T_51_001.c001
  is '��¼��ˮ�ţ�1 - 2147483647 (21��)';
comment on column T_51_001.c002
  is '¼����ˮ��';
comment on column T_51_001.c003
  is '��ʼ¼��ʱ�䣬¼��������UTCʱ��';
comment on column T_51_001.c004
  is '����¼��ʱ�䣬¼��������UTCʱ��';
comment on column T_51_001.c005
  is '�⻧���';
comment on column T_51_001.c006
  is '¼����������';
comment on column T_51_001.c007
  is '¼��������IP';
comment on column T_51_001.c008
  is '��ϯ��';
comment on column T_51_001.c009
  is '�ֻ���(VMC������ֵ)';
comment on column T_51_001.c010
  is '¼����չ������ϵͳ���ã����ֵ������д�磺�ֻ�����ϯ�ŵ�';
comment on column T_51_001.c011
  is '�к�';
comment on column T_51_001.c012
  is '��ֵ������ 1 �� 100000';
comment on column T_51_001.c013
  is '������0��ȫ����1����������2��������';
