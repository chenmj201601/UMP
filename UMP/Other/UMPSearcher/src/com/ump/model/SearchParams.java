package com.ump.model;

public class SearchParams {
	private String dtstart; // ¼����ʼʱ��
	private String dtend; // ¼������ʱ��
	private String callno; // ���к���
	private String calledno;// ���к���
	private String direction;// ���з���
	private String agent;// ��ϯ
	private String extension;// �ֻ�
	private String rentid; //�⻧ID
	private String rentname;//�⻧��
	private String recordreference;// ��¼
	private String lenstart;// ��ʼʱ��
	private String lenend;// ����ʱ��

	public String getLenstart() {
		return lenstart;
	}

	public void setLenstart(String lenstart) {
		this.lenstart = lenstart;
	}

	public String getLenend() {
		return lenend;
	}

	public void setLenend(String lenend) {
		this.lenend = lenend;
	}

	public SearchParams() {
		super();
	}

	public SearchParams(String dtstart, String dtend, String callno,
			String calledno, String direction, String agent, String extension,
			String rentid, String rentname, String ref, String lenstart,
			String lenend) {
		super();
		this.dtstart = dtstart;
		this.dtend = dtend;
		this.callno = callno;
		this.calledno = calledno;
		this.direction = direction;
		this.agent = agent;
		this.extension = extension;
		this.rentid = rentid;
		this.rentname = rentname;
		this.recordreference = ref;
		this.lenstart = lenstart;
		this.lenend = lenend;
	}

	public String getAgent() {
		return agent;
	}

	public void setAgent(String agent) {
		this.agent = agent;
	}

	public String getExtension() {
		return extension;
	}

	public void setExtension(String extension) {
		this.extension = extension;
	}

	public String getDtstart() {
		return dtstart;
	}

	public void setDtstart(String dtstart) {
		this.dtstart = dtstart;
	}

	public String getDtend() {
		return dtend;
	}

	public void setDtend(String dtend) {
		this.dtend = dtend;
	}

	public String getCallno() {
		return callno;
	}

	public void setCallno(String callno) {
		this.callno = callno;
	}

	public String getCalledno() {
		return calledno;
	}

	public void setCalledno(String calledno) {
		this.calledno = calledno;
	}

	public String getDirection() {
		return direction;
	}

	public void setDirection(String direction) {
		this.direction = direction;
	}

	public String getRentid() {
		return rentid;
	}

	public void setRentid(String rentid) {
		this.rentid = rentid;
	}

	public String getRentname() {
		return rentname;
	}

	public void setRentname(String rentname) {
		this.rentname = rentname;
	}

	public String getRecordreference() {
		return recordreference;
	}

	public void setRecordreference(String recordreference) {
		this.recordreference = recordreference;
	}

}
