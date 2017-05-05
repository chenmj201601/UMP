package com.ump.util;

import java.io.*;
import java.net.*;
import java.util.*;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

public class DownloadUtil {
	/**
	 * ����
	 * 
	 * @param upath
	 *            UMP mediadata�е�MP3����wav�ʼ�·��
	 * @param propath
	 *            JAVA����·��
	 * @param filename
	 *            �µ����ļ���
	 * @param localpath
	 *            ������client���ص�·��
	 * @return
	 */
	public static boolean downFiletoLocal(String upath, String propath,
			String filename, String localpath, HttpServletRequest request,
			HttpServletResponse response) {
		boolean ret = false;
		DelFilesOneDayAgo(propath);
		if (saveToFile(upath, propath + filename))// ��UMP���ص�JAVA����·���ɹ�
		{
			ret = true;
		}
		// ��һ�� �����ļ���JAVA��������Ŀ¼

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
			long time = file.lastModified();// �����ļ�����޸�ʱ�䣬���Ը�long�ͺ�����
			Date dtfileDate = new Date(time);

			Calendar cd = Calendar.getInstance(); 
			cd.add(Calendar.DATE, -1);
			Date dtoneday=cd.getTime(); 
			long oneday = dtoneday.getTime();
			long filedt = dtfileDate.getTime();
			
			if(oneday > filedt)//�ļ���һ��ǰ����
			{
				file.delete();
			}
		}
	}

	public static void main(String[] args) {
		DelFilesOneDayAgo("C:\\Program Files\\Apache Software Foundation\\Tomcat 7.0\\webapps\\UMPSearcher\\temp");
	}
}
