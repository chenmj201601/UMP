CREATE PROCEDURE P_31_002  (
   @AInParam01              VARCHAR(5) = '00000',  
   @AInParam02              VARCHAR(1) = 'I',
   @AInParam03              VARCHAR(20)  output,
   @AInParam04              VARCHAR(20),
   @AInParam05              VARCHAR(20),
   @AInParam06              VARCHAR(20),
   @AInParam07              VARCHAR(20),
   @AInParam08              VARCHAR(128),
   @AInParam09              VARCHAR(1024),
   @AInParam10              VARCHAR(1) ='A',
   @AInParam11              VARCHAR(1) ='1',
   @AInParam12              VARCHAR(20),
   @AInParam13              VARCHAR(20),
   @AInParam14              VARCHAR(20)=null,
   @AInParam15              VARCHAR(20)=null,
   @AInParam16              VARCHAR(20)=null,
   @AInParam17              VARCHAR(20)=null,
   @AInParam18              VARCHAR(20)= '0',     
   @AInParam19              VARCHAR(1),
   @AInParam20              VARCHAR(20)='0',
   @AInParam21              VARCHAR(1)='1',               
   @AOutErrorNumber         BIGINT          OUTPUT,
   @AOutErrorString         NVARCHAR(4000)  OUTPUT
)
AS
-- description:    增、删、改表T_31_042_00000录音记录的BookMark
-- create date:    <2014-11-05>
-- =========================================================================================
--AInParam01        租户标识（默认为‘00000’）
--AInParam02        操作标识 I： 增加 D：删除 U：修改
--AInParam03        Book Mark 流水号
--AInParam04        录音记录流水号.C001
--AInParam05        录音记录流水号.C002
--AInParam06        OffsetTime.与起点的偏移量，单位：毫秒
--AInParam07        时长。
--AInParam08        主题
--AInParam09        内容
--AInParam10        共享方式 默认为'A'
--AInParam11        状态。'0'删除，'1'正常
--AInParam12        标注人ID
--AInParam13        标注时间 yyyyMMddHHmmss
--AInParam14        最后修改人
--AInParam15        最后修改时间 yyyyMMddHHmmss
--AInParam16        删除人
--AInParam17        删除时间 yyyyMMddHHmmss
--AInParam18        级别 默认0
--AInParam19        来源.'C'从CQC写入，'U'从UMP写入
--AInParam20        信誉值
--AInParam21        通道号

   DECLARE @v_serialid     VARCHAR(19);
   DECLARE @v_AInParam02   CHAR(1);
   DECLARE @v_err_num      BIGINT;
   DECLARE @v_err_msg      NVARCHAR (200);
   DECLARE @v_execsql      NVARCHAR (4000);
   DECLARE @row            BIGINT; --受影响的行
   

BEGIN try

   SET @row =0;

   IF (@AInParam09 IS  NULL) OR (@AInParam09='')
   BEGIN
     SET @AInParam09=''
   END

   SET @v_AInParam02 =UPPER(@AInParam02)

   IF @v_AInParam02 NOT IN ('I','D','U') 

   BEGIN
        SET @AOuterrornumber = '-1';
        SET @AOuterrorstring = 'AInParam02 is illegal';
        RETURN;
   END;
   
   IF  @v_AInParam02 IN  ('D','U') AND (@AInParam03 IS NULL OR @AInParam03='')
   BEGIN
        SET @AOuterrornumber = '-1';
        SET @AOuterrorstring = 'AInParam03 is illegal';
        RETURN;     
   END
 
   IF    @v_AInParam02 IN  ('I','U') AND ((@AInParam04 IS NULL OR @AInParam04='') OR  (@AInParam05 IS NULL OR @AInParam05='')  OR  (@AInParam06 IS NULL OR @AInParam06='')) 
   BEGIN
        SET @AOuterrornumber = '-1';
        SET @AOuterrorstring = 'AInParam04 or AInParam05 or AInParam06 is illegal';
        RETURN;     
   END

   IF   @v_AInParam02 in  ('I','U') and ((@AInParam07 is null or @AInParam07='') OR  (@AInParam08 is null or @AInParam08='')  OR  (@AInParam12 is null or @AInParam12='') OR (@AInParam13 is null or @AInParam13='')  OR (@AInParam19 is null or @AInParam19=''))
   BEGIN
        SET @AOuterrornumber = '-1';
        SET @AOuterrorstring = 'AInParam07 or AInParam08 or AInParam12 or AInParam13 or AInParam19 is illegal';
        RETURN;     
   END
         
   --如果操作编码是I增加记录
   IF @v_AInParam02='I'  
   BEGIN
        --模块号是31 操作码是142
        EXEC p_00_001  '31','304',@ainparam01, DEFAULT,@v_serialid OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT

        IF (CONVERT(BIGINT, @v_serialid)<0 OR  @v_err_num<>0 )
        BEGIN
          SET @AOutErrorNumber=CONVERT(BIGINT, @v_serialid)
          SET @AOutErrorString='Serial number generation error'
          RETURN
        END

        SET @v_execsql =
          'INSERT INTO T_31_042_'
          + @AInParam01
          + '
          VALUES ('
          + @v_serialid
          + ','
          + @AInParam04
          + ','
          + @AInParam05
          + ','
          + @AInParam06
          + ','
          + @AInParam07
          + ', N'''
          + @AInParam08
          + ''',N'''
          + @AInParam09
          + ''','''
          + @AInParam10
          + ''','''
          + @AInParam11
          + ''','''
          + @AInParam12
          + ''','
          + @AInParam13
          + ',';
    --print @v_execsql

    IF ( @AInParam14 IS NOT NULL) AND (@AInParam14<>'')
      SET @v_execsql=@v_execsql+ @AInParam14+','
    ELSE
      SET @v_execsql=@v_execsql+'NULL,';


    if (@AInParam15 IS NOT NULL) and (@AInParam15<>'')
      SET @v_execsql=@v_execsql+ @AInParam15+','
    ELSE
      SET @v_execsql=@v_execsql+'NULL,';


    IF (@AInParam16 IS NOT NULL) and (@AInParam16<>'')
      SET @v_execsql=@v_execsql+ @AInParam16+','
    ELSE
      SET @v_execsql=@v_execsql+'NULL,';


    IF (@AInParam17 IS NOT NULL) AND (@AInParam17<>'')
      SET @v_execsql=@v_execsql+ @AInParam17+','
    ELSE
      SET @v_execsql=@v_execsql+'NULL,';


     SET @v_execsql=@v_execsql
          + @AInParam18
          + ','''
          + @AInParam19 
          + ','
          + @AInParam20
          + ','''
          + @AInParam21		  	                                                                                           
          + ''')';
        --print @v_execsql
        EXEC SP_EXECUTESQL @v_execsql;
     SET @row = @@rowcount 
     PRINT @row

     IF @row=1 
     BEGIN
      SET @AInParam03=@v_serialid;
          SET @AOutErrornumber = '0';
          SET @AOutErrorstring = 'successful';
    END;
    ELSE
    BEGIN
          SET @AOutErrornumber = '-1';
          SET @AOutErrorstring = 'insert faild';
    END;

    END;

    IF  @v_AInParam02='D'  
    BEGIN
         SET @v_execsql =
          'DELETE t_31_042_'
          +@AInParam01
          + '
          WHERE C001='+@AInParam03;
          --print @v_execsql;
          EXEC SP_EXECUTESQL @v_execsql;
      SET @row=@@ROWCOUNT
      print @row
      IF @row=1 
      BEGIN
        SET @AOutErrornumber = '0';
        SET @AOutErrorstring = 'successful';
      END;
      ELSE
      BEGIN
        SET @AOutErrornumber = '-1';
        SET @AOutErrorstring = 'delete faild';
      END;

   END
           
   IF  @v_AInParam02='U' 
   BEGIN
   
          SET @v_execsql =
          'UPDATE T_31_042_'
          +@AInParam01
          + '
          SET  C002='''
          + @AInParam04
          + ''',C003='''
          + @AInParam05  
          + ''',C004='''
          + @AInParam06          
          + ''',C005='''
          + @AInParam07
          + ''',C006=N'''
          + @AInParam08
          + ''',C007=N'''
          + @AInParam09
          + ''',C008='''
          + @AInParam10
          + ''',C009='''
          + @AInParam11
          + ''',C010='''
          + @AInParam12
          + ''',C011='''
          + @AInParam13
          +''''

      IF (@AInParam14 IS NOT NULL) OR (@AInParam14<>'')
      BEGIN
            SET @v_execsql =@v_execsql+ ',C012='''+@AInParam14+''''
      END
      ELSE
      BEGIN
        SET @v_execsql =@v_execsql+', C012=null'
      END

      IF (@AInParam15 IS NOT NULL) OR (@AInParam15<>'')
      BEGIN
            SET @v_execsql=@v_execsql + ',C013='''+@AInParam15+''''
      END
      ELSE
      BEGIN
            SET @v_execsql =@v_execsql+', C013=NULL'
      END


      IF (@AInParam16 IS NOT NULL) OR (@AInParam16<>'')
      BEGIN
            SET @v_execsql=@v_execsql + ',C014='''+@AInParam16+''''
      END
      ELSE
      BEGIN
            SET @v_execsql =@v_execsql+', C014=NULL'
      END

      IF (@AInParam17 IS NOT NULL) OR (@AInParam17<>'')
      BEGIN
            SET @v_execsql=@v_execsql + ',C015='''+@AInParam17+''''
      END
      ELSE
      BEGIN
            SET @v_execsql =@v_execsql+', C015=NULL'
      END

          SET @v_execsql =@v_execsql+ ',C016='''
          + @AInParam18
          + ''',C017='''
          + @AInParam19
          + ''',C018='
          + @AInParam20
          + ',C019='''
          + @AInParam21		  		                                                                                       
          + ''' WHERE C001='+@AInParam03;
          --print @v_execsql;
          EXEC SP_EXECUTESQL @v_execsql;

      SET @row = @@rowcount 
      print @row
      IF @row=1 
      BEGIN
            SET @AOutErrornumber = '0';
            SET @AOutErrorstring = 'successful';
      END;
      ELSE
      BEGIN
            SET @AOutErrornumber = '-1';
            SET @AOutErrorstring = 'update faild';
      END;
 
   END;

END TRY
BEGIN CATCH
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH