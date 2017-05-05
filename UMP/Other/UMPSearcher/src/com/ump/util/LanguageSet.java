package com.ump.util;

import java.io.*;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

public class LanguageSet {
	
	private String mCurrentLan;
	private String mpath3;

	public LanguageSet(String mCurrentLan, String path3) {
		this.mCurrentLan = mCurrentLan;
		this.mpath3 = path3;
	}

	public String getMpath3() {
		return mpath3;
	}

	public void setMpath3(String mpath3) {
		this.mpath3 = mpath3;
	}

	public String getmCurrentLan() {
		return mCurrentLan;
	}

	public void setmCurrentLan(String mCurrentLan) {
		this.mCurrentLan = mCurrentLan;
	}

	@SuppressWarnings("unused")
	public String[] LanguagePara() {
		String ParaName[] = null;// ��������

		ParaName = new String[50];

		String sLanguaeSet;
		String sLanguaecode;

		sLanguaeSet = "";
		sLanguaecode = "";

		String relativelyPath = System.getProperty("user.dir");
		StringBuilder sBuilder = new StringBuilder();
		sBuilder.append(mpath3).append("\\").append("Language.xml");

		String xmlfilepath = sBuilder.toString();

		Element element = null;
		// ����ʹ�þ���·��
		File f = new File(xmlfilepath);
		// documentBuilderΪ������ֱ��ʵ����(��XML�ļ�ת��ΪDOM�ļ�)
		DocumentBuilder db = null;
		DocumentBuilderFactory dbf = null;

		try {
			// ����documentBuilderFactory����
			dbf = DocumentBuilderFactory.newInstance();
			// ����db������documentBuilderFatory�����÷���documentBuildr����
			db = dbf.newDocumentBuilder();
			// �õ�һ��DOM�����ظ�document����
			Document dt = db.parse(f);
			// �õ�һ��elment��Ԫ��
			element = dt.getDocumentElement();

			// ��ø��ڵ�
			// System.out.println("��Ԫ�أ�" + element.getNodeName());
			if (mCurrentLan == null || mCurrentLan.equals(""))
				sLanguaeSet = element.getAttributes().getNamedItem("type")
						.getNodeValue();
			else
				sLanguaeSet = mCurrentLan;
			// System.out.println("ȷ�����ԣ�" + sLanguaeSet);
			// ��ø�Ԫ���µ��ӽڵ�
			NodeList childNodes = element.getChildNodes();

			// ������Щ�ӽڵ�
			for (int i = 0; i < childNodes.getLength(); i++) {
				// ���ÿ����Ӧλ��i�Ľ��
				Node node1 = childNodes.item(i);
				if ("Account".equals(node1.getNodeName())) {
					// ����ڵ������Ϊ"Account"�������AccountԪ������type
					/*
					 * System.out.println("\r\n�ҵ�һƪ�˺�. ����: " +
					 * node1.getAttributes().getNamedItem("type")
					 * .getNodeValue() + ". ");
					 */
					sLanguaecode = node1.getAttributes().getNamedItem("type")
							.getNodeValue();

					if (sLanguaecode.equals(sLanguaeSet))
					// ���<Accounts>�µĽڵ�
					{
						NodeList nodeDetail = node1.getChildNodes();
						// ����<Accounts>�µĽڵ�
						int paramelen = 0;
						for (int j = 0; j < nodeDetail.getLength(); j++) {
							int length = nodeDetail.getLength();
							// ���<Accounts>Ԫ��ÿһ���ڵ�
							Node detail = nodeDetail.item(j);
							String nodenameString = detail.getNodeName();
							if (nodenameString != null
									&& nodenameString.length() > 3
									&& nodenameString.substring(0, 2).equals(
											"LL")) // ���username
							{
								ParaName[paramelen] = detail.getTextContent();
								paramelen++;
							}
						}
					}
				}

			}
		}

		catch (Exception e) {
			e.printStackTrace();
		}
		return ParaName;
	}
}
