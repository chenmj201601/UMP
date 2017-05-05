package com.ump.database;

import java.io.BufferedReader;
import java.io.FileReader;

public class XMLUtil {

	public XMLUtil() {
	}

	public static String getTemplateFile(String s) {
		String s1 = new String();
		try {
			FileReader filereader = new FileReader(s);
			BufferedReader bufferedreader = new BufferedReader(filereader);
			do {
				String s2 = bufferedreader.readLine();
				if (s2 == null)
					break;
				if (s1 != null)
					s1 = s1 + "\n";
				s1 = s1 + s2;
			} while (true);
			bufferedreader.close();
		} catch (Exception exception) {
			return new String();
		}
		return s1;
	}

	public static String xmlTemplateReplace(String s, String s1, String s2) {
		return replaceFirstFindString(s, s1, s2);
	}

	public static String xmlTemplateReplaceWithShift(String s, String s1,
			String s2) {
		s2 = stringToXMLString(s2);
		return replaceFirstFindString(s, s1, s2);
	}

	public static String buildXMLString(String s, String s1) {
		return "<" + s + ">" + stringToXMLString(s1) + "</" + s + ">\n";
	}

	public static String stringToXMLString(String s) {
		String s1 = new String();
		if (s == null)
			return new String();
		for (int i = 0; i < s.length(); i++) {
			char c = s.charAt(i);
			if (c == '&')
				s1 = s1 + "&amp;";
			else if (c == '<')
				s1 = s1 + "&lt;";
			else if (c == '>')
				s1 = s1 + "&gt;";
			else if (c == '"')
				s1 = s1 + "&quot;";
			else if (c == '\'')
				s1 = s1 + "&apos;";
			else
				s1 = s1 + c;
		}

		return s1;
	}

	public static String xmlStringToString(String s) {
		if (s == null) {
			return new String();
		} else {
			String s1 = replaceAllFindString(s, "&amp;", "huadi-idauh");
			s1 = replaceAllFindString(s1, "&lt;", "<");
			s1 = replaceAllFindString(s1, "&gt;", ">");
			s1 = replaceAllFindString(s1, "&quot;", "\"");
			s1 = replaceAllFindString(s1, "&apos;", "'");
			s1 = replaceAllFindString(s, "huadi-idauh", "&");
			return s1;
		}
	}

	@SuppressWarnings("unused")
	public static String replaceFirstFindString(String s, String s1, String s2) {
		String s3 = new String();
		if (s == null)
			return new String();
		if (s1 == null || s2 == null)
			return s;
		int i = s.indexOf(s1);
		if (i < 0) {
			return s;
		} else {
			String s4 = s.substring(0, i) + s2 + s.substring(i + s1.length());
			return s4;
		}
	}

	public static String replaceAllFindString(String s, String s1, String s2) {
		String s3 = new String();
		if (s == null)
			return new String();
		if (s1 == null || s2 == null)
			return s;
		do {
			int i = s.indexOf(s1);
			if (i >= 0) {
				s3 = s3 + s.substring(0, i) + s2;
				s = s.substring(i + s1.length());
			} else {
				s3 = s3 + s;
				return s3;
			}
		} while (true);
	}

	public static String getContentByTag(String s, String s1) {
		String s2 = new String();
		if (s == null || s1 == null)
			return new String();
		String s4 = "<" + s1 + ">";
		String s5 = "</" + s1 + ">";
		int i = s.indexOf(s4);
		if (i < 0)
			return s2;
		int j = s.indexOf(s5);
		if (j < 0) {
			return s2;
		} else {
			String s3 = s.substring(i + s4.length(), j);
			return s3;
		}
	}
}
