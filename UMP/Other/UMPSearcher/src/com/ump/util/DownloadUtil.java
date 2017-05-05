package com.ump.util;

import java.io.*;
import java.net.*;
import java.util.*;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

public class DownloadUtil {
	/**
	 * 下载
	 * 
	 * @param upath
	 *            UMP mediadata中的MP3或者wav问价路径
	 * @param propath
	 *            JAVA工程路径
	 * @param filename
	 *            新导出文件名
	 * @param localpath
	 *            导出到client本地的路径
	 * @return
	 */
	public static boolean downFiletoLocal(String upath, String propath,
			String filename, String localpath, HttpServletRequest request,
			HttpServletResponse response) {
		boolean ret = false;
		DelFilesOneDayAgo(propath);
		if (saveToFile(upath, propath + filename))// 从UMP下载到JAVA工程路径成功
		{
			ret = true;
		}
		// 第一步 下载文件到JAVA工程所在目录

		return ret;
	}

	private static boolean saveToFile(String sRemoteHttpURL,
			String sLocalSaveFile) {
		if (sRemoteHttpURL == null || sRemoteHttpURL.trim().equals("")) {
			return false;
		}

		try {
			URL tURL = new URL(sRemoteHttpURL);
			HttpURLConnection tHttpURLConnection = (HttpURLConnection) tURL
					.openConnection();
			tHttpURLConnection.connect();
			BufferedInputStream tBufferedInputStream = new BufferedInputStream(
					tHttpURLConnection.getInputStream());
			FileOutputStream tFileOutputStream = new FileOutputStream(
					sLocalSaveFile);

			int nBufferSize = 1024 * 5;
			byte[] bufContent = new byte[nBufferSize];
			int nContentSize = 0;
			while ((nContentSize = tBufferedInputStream.read(bufContent)) != -1) {
				tFileOutputStream.write(bufContent, 0, nContentSize);
			}

			tFileOutputStream.close();
			tBufferedInputStream.close();
			tHttpURLConnection.disconnect();

			tURL = null;
			tHttpURLConnection = null;
			tBufferedInputStream = null;
			tFileOutputStream = null;
		} catch (Exception ex) {
			return false;
		}
		return true;
	}

	public static void DelFilesOneDayAgo(String dir) {
		File root = new File(dir);
		File[] files = root.listFiles();
		for (File file : files) {
			long time = file.lastModified();// 返回文件最后修改时间，是以个long型毫秒数
			Date dtfileDate = new Date(time);

			Calendar cd = Calendar.getInstance(); 
			cd.add(Calendar.DATE, -1);
			Date dtoneday=cd.getTime(); 
			long oneday = dtoneday.getTime();
			long filedt = dtfileDate.getTime();
			
			if(oneday > filedt)//文件在一天前生成
			{
				file.delete();
			}
		}
	}

	public static void main(String[] args) {
		DelFilesOneDayAgo("C:\\Program Files\\Apache Software Foundation\\Tomcat 7.0\\webapps\\UMPSearcher\\temp");
	}
}
