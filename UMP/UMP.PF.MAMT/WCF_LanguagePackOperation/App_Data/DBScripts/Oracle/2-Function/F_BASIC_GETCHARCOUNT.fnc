create or replace function F_BASIC_GETCHARCOUNT(p_sentence in varchar2, p_char in varchar2) return number is
  Result number:=0; --确定字符串中有几个指定字符
  i int:=1;
begin
  loop
     if  instr(p_sentence,p_char,1,i)>0 then
         result:=result+1;
     end if;
     i:=i+1;
     exit when instr(p_sentence,p_char,1,i)=0;
  end loop;
  return(Result);
end;
/
