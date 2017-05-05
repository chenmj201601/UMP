package com.ump.model;

public class SearchParams {
	private String dtstart; // 录音开始时间
	private String dtend; // 录音结束时间
	private String callno; // 主叫号码
	private String calledno;// 被叫号码
	private String direction;// 呼叫方向
	private String agent;// 坐席
	private String extension;// 分机
	private String rentid; //租户ID
	private String rentname;//租户名
	private String recordreference;// 记录
	private String lenstart;// 开始时长
	private String lenend;// 结束时长

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
