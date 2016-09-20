create or replace procedure P_00_001(AInParam01      varchar2,
                                     AInParam02      varchar2,
                                     AInParam03      varchar2:='00000',
                                     AInParam04      varchar2:='20140101000000',
                                     AOutParam01     out varchar2,
                                     AOutErrorNumber out number,
                                     AOutErrorString out nvarchar2)

  is


-- type:           ϵͳ��ˮ�����ɹ��̣���¼Ŀ���ΪT_00_001
-- create date:    <2014-08-6>
-- description:    <��ȡϵͳ�Զ�����>
-- =========================================================================================
-- parameters description:
-- @ainparam01:   ģ�����
-- @ainparam02:   ģ���ڱ��� 100 �� 920
-- @ainparam03:   �⻧���,Ĭ��Ϊ00000
-- @ainparam04:   ʱ����� �̶�ʱ���ʼ¼��ʱ��
-- =========================================================================================
-- return description:
-- @AOutParam01:�ɹ��󷵻ص�ϵͳ�Զ����bigint��
--          -10��ģ��������
--          -11��ģ�鲻���ڣ�
--          -20���ڲ��������
--          -21���ڲ����뷶Χ����
--          100yymmddhh00000000:�ɹ��󷵻ص�ϵͳ�Զ����varchar��
-- ==========================================================================================
    v_Ainparam03     varchar2(5);
    v_Ainparam04     varchar2(14);
    v_err_num        number;
    v_err_msg        nvarchar2(200);
    vcurrenthour     varchar2(8);
    intserialnumber  number(11);
    intdatacount     number(11);
    intcurrentnumber number(11);
    intmoduleid      number(6);
    intserialtype    number(6);
    v_sql            varchar2(500);


begin
   if Ainparam03 is null then
     v_Ainparam03:='00000';
   else
     v_Ainparam03:=Ainparam03;
   end if;

   if Ainparam04 is null then
     v_Ainparam04:='20140101000000';
   else
     v_Ainparam04:=Ainparam04;
   end if;

   if f_basic_isnumeric(ainparam01) = 0 then
        AOutErrorNumber := -10;
        AOutErrorString:='failed';
        return;
    end if;

    if f_basic_isnumeric(ainparam02) = 0 then
        AOutErrorNumber := -20;
        AOutErrorString:='failed';        
        return;
    end if;

    intmoduleid   := to_number(ainparam01);
    intserialtype := to_number(ainparam02);

    if (intmoduleid < 11 or intmoduleid > 91) then
        AOutErrorNumber := -10;
        AOutErrorString:='failed';        
        return;
    end if;

    if (intserialtype < 100 or intserialtype > 999) then
        AOutErrorNumber := -21;
        AOutErrorString:='failed';              
        return;
    end if;

    v_sql:='select count(1) from t_00_008 where c001 = '||intmoduleid;

    execute immediate v_sql into intdatacount;

    if intdatacount = 0 then
        AOutErrorNumber := -11; 
        AOutErrorString:='failed';             
        return;
    end if;   

    vcurrenthour := substr(v_Ainparam04,3,8);

    intserialnumber := to_number(vcurrenthour);

    select count(1)
      into intdatacount
      from t_00_001
     where c000 =v_Ainparam03
       and c001 = intmoduleid
       and c002 = intserialtype
       and c003 = intserialnumber;

    if intdatacount = 0 then
        insert into t_00_001
        values(v_ainparam03,intmoduleid,intserialtype, intserialnumber,0);
    end if;

    select nvl(c004,0) + 1
      into intcurrentnumber
      from t_00_001
     where c000 =v_Ainparam03
       and c001 = intmoduleid
       and c002 = intserialtype
       and c003 = intserialnumber;

    update t_00_001
       set c004 = intcurrentnumber
     where c000 =v_Ainparam03
       and c001 = intmoduleid
       and c002 = intserialtype
       and c003 = intserialnumber;

    commit;
    --��ʽΪ  100:�������Ψһ yymmddhh:Сʱʱ�� 00000000��8λ��ˮ��   3+8+8 19λ
        
    AOutParam01 := ainparam02||to_char(intserialnumber*100000000+intcurrentnumber);
    AOutErrorNumber :=0;
    AOutErrorString :='successful';

exception
  when others then
    rollback;
    v_err_num := sqlcode;
    v_err_msg  := substr(sqlerrm,1,200);
    AOutErrorNumber := v_err_num;
    AOutErrorString := v_err_msg;
end;
/
