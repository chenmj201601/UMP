package com.ump.util;

import java.io.*;  
import java.util.zip.*; 

public class ZipFiles {
	
	private int k = 1; // 定义递归次数变量  
	  
    public ZipFiles() {  
    }  

    private void zip(String zipFileName, File inputFile) throws Exception {  
        System.out.println("压缩中...");  
        ZipOutputStream out = new ZipOutputStream(new FileOutputStream(  
                zipFileName));  
        BufferedOutputStream bo = new BufferedOutputStream(out);  
        zip(out, inputFile, inputFile.getName(), bo);  
        bo.close();  
        out.close(); // 输出流关闭  
        System.out.println("压缩完成");  
    }  
    
    private void zip(ZipOutputStream out, File f, String base,  
            BufferedOutputStream bo) throws Exception { // 方法重载  
        if (f.isDirectory()) {  
            File[] fl = f.listFiles();  
            if (fl.length == 0) {  
                out.putNextEntry(new ZipEntry(base + "/")); // 创建zip压缩进入点base  
                System.out.println(base + "/");  
            }  
            for (int i = 0; i < fl.length; i++) {  
                zip(out, fl[i], base + "/" + fl[i].getName(), bo); // 递归遍历子文件夹  
            }  
            System.out.println("第" + k + "次递归");  
            k++;  
        } else {  
            out.putNextEntry(new ZipEntry(base)); // 创建zip压缩进入点base  
            System.out.println(base);  
            FileInputStream in = new FileInputStream(f);  
            BufferedInputStream bi = new BufferedInputStream(in);  
            int b;  
            while ((b = bi.read()) != -1) {  
                bo.write(b); // 将字节流写入当前zip目录  
            }  
            bi.close();  
            in.close(); // 输入流关闭  
        }  
    }  
    
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		ZipFiles book = new ZipFiles();  
        try {  
            book.zip("C:\\Program Files\\Apache Software Foundation\\Tomcat 7.0\\webapps\\UMPSearcher\\zip\\Download.zip",  
                    new File("C:\\Program Files\\Apache Software Foundation\\Tomcat 7.0\\webapps\\UMPSearcher\\temp"));  
        } catch (Exception e) {  
            // TODO Auto-generated catch block  
            e.printStackTrace();  
        }  
	}

}
