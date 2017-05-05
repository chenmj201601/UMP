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
		String ParaName[] = null;// 声明数组

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
		// 可以使用绝对路劲
		File f = new File(xmlfilepath);
		// documentBuilder为抽象不能直接实例化(将XML文件转换为DOM文件)
		DocumentBuilder db = null;
		DocumentBuilderFactory dbf = null;

		try {
			// 返回documentBuilderFactory对象
			dbf = DocumentBuilderFactory.newInstance();
			// 返回db对象用documentBuilderFatory对象获得返回documentBuildr对象
			db = dbf.newDocumentBuilder();
			// 得到一个DOM并返回给document对象
			Document dt = db.parse(f);
			// 得到一个elment根元素
			element = dt.getDocumentElement();

			// 获得根节点
			// System.out.println("根元素：" + element.getNodeName());
			if (mCurrentLan == null || mCurrentLan.equals(""))
				sLanguaeSet = element.getAttributes().getNamedItem("type")
						.getNodeValue();
			else
				sLanguaeSet = mCurrentLan;
			// System.out.println("确定语言：" + sLanguaeSet);
			// 获得根元素下的子节点
			NodeList childNodes = element.getChildNodes();

			// 遍历这些子节点
			for (int i = 0; i < childNodes.getLength(); i++) {
				// 获得每个对应位置i的结点
				Node node1 = childNodes.item(i);
				if ("Account".equals(node1.getNodeName())) {
					// 如果节点的名称为"Account"，则输出Account元素属性type
					/*
					 * System.out.println("\r\n找到一篇账号. 语言: " +
					 * node1.getAttributes().getNamedItem("type")
					 * .getNodeValue() + ". ");
					 */
					sLanguaecode = node1.getAttributes().getNamedItem("type")
							.getNodeValue();

					if (sLanguaecode.equals(sLanguaeSet))
					// 获得<Accounts>下的节点
					{
						NodeList nodeDetail = node1.getChildNodes();
						// 遍历<Accounts>下的节点
						int paramelen = 0;
						for (int j = 0; j < nodeDetail.getLength(); j++) {
							int length = nodeDetail.getLength();
							// 获得<Accounts>元素每一个节点
							Node detail = nodeDetail.item(j);
							String nodenameString = detail.getNodeName();
							if (nodenameString != null
									&& nodenameString.length() > 3
									&& nodenameString.substring(0, 2).equals(
											"LL")) // 输出username
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
