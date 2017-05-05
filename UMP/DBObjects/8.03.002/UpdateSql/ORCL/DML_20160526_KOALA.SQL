--将录音通道参数的两个默认值做一下修改：DTMFTimeout的默认值改为2000，StartStopDelay的默认值改回100 koala 20160526
update T_00_009 set C009 = '100'  where C001 = 225 and C002 = 24;
update T_00_009 set C009 = '2000' where C001 = 225 and C002 = 25;