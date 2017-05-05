package com.ump.model;

public class RecordData {
	
	private String serialid;
	private String startrecordtime;
	private String voiceid;
	private String voiceip;
	private String chanel;
	private String extension;
	
	private String agent;
	private String recordlength;
	private String direction;
	private String callno;
	private String calledno;
	private String recordreference;
	
	private String isscored;
	private String skillgroup;
	private String realextension;
	private String thirdno;
	private String stoprecordtime;
	private String islabel;
	
	private String isbookmark;
	private String chanelname;
	
	private String isencrypt;

	public RecordData() {
		super();
	}

	public RecordData(String serialid,String startrecordtime,String voiceid,String voiceip,String chanel,String extension,
			String agent,String recordlength,String direction,String callno,String calledno,String recordreference,
			String isscored,String skillgroup,String realextension,String thirdno,String stoprecordtime,String islabel,
			String isbookmark,String chanelname,String isencrypt) {
		super();
		this.serialid = serialid;
		this.startrecordtime = startrecordtime;
		this.voiceid = voiceid;
		this.voiceip = voiceip;
		this.chanel = chanel;
		this.extension = extension;
		
		this.agent = agent;
		this.recordlength = recordlength;
		this.direction = direction;
		this.callno = callno;
		this.calledno = calledno;
		this.recordreference = recordreference;
		
		this.isscored = isscored;
		this.skillgroup = skillgroup;
		this.realextension = realextension;
		this.thirdno = thirdno;
		this.stoprecordtime = stoprecordtime;
		this.islabel = islabel;
		
		this.isbookmark = isbookmark;
		this.chanelname = chanelname;
		this.isencrypt = isencrypt;
	}

	public String getSerialid() {
		return serialid;
	}

	public void setSerialid(String serialid) {
		this.serialid = serialid;
	}

	public String getStartrecordtime() {
		return startrecordtime;
	}

	public void setStartrecordtime(String startrecordtime) {
		this.startrecordtime = startrecordtime;
	}

	public String getVoiceid() {
		return voiceid;
	}

	public void setVoiceid(String voiceid) {
		this.voiceid = voiceid;
	}

	public String getVoiceip() {
		return voiceip;
	}

	public void setVoiceip(String voiceip) {
		this.voiceip = voiceip;
	}

	public String getChanel() {
		return chanel;
	}

	public void setChanel(String chanel) {
		this.chanel = chanel;
	}

	public String getExtension() {
		return extension;
	}

	public void setExtension(String extension) {
		this.extension = extension;
	}

	public String getAgent() {
		return agent;
	}

	public void setAgent(String agent) {
		this.agent = agent;
	}

	public String getRecordlength() {
		return recordlength;
	}

	public void setRecordlength(String recordlength) {
		this.recordlength = recordlength;
	}

	public String getDirection() {
		return direction;
	}

	public void setDirection(String direction) {
		this.direction = direction;
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

	public String getRecordreference() {
		return recordreference;
	}

	public void setRecordreference(String recordreference) {
		this.recordreference = recordreference;
	}

	public String getIsscored() {
		return isscored;
	}

	public void setIsscored(String isscored) {
		this.isscored = isscored;
	}

	public String getSkillgroup() {
		return skillgroup;
	}

	public void setSkillgroup(String skillgroup) {
		this.skillgroup = skillgroup;
	}

	public String getRealextension() {
		return realextension;
	}

	public void setRealextension(String realextension) {
		this.realextension = realextension;
	}

	public String getThirdno() {
		return thirdno;
	}

	public void setThirdno(String thirdno) {
		this.thirdno = thirdno;
	}

	public String getStoprecordtime() {
		return stoprecordtime;
	}

	public void setStoprecordtime(String stoprecordtime) {
		this.stoprecordtime = stoprecordtime;
	}

	public String getIslabel() {
		return islabel;
	}

	public void setIslabel(String islabel) {
		this.islabel = islabel;
	}

	public String getIsbookmark() {
		return isbookmark;
	}

	public void setIsbookmark(String isbookmark) {
		this.isbookmark = isbookmark;
	}

	public String getChanelname() {
		return chanelname;
	}

	public void setChanelname(String chanelname) {
		this.chanelname = chanelname;
	}	
	public String getIsencrypt() {
		return isencrypt;
	}

	public void setIsencrypt(String isencrypt) {
		this.isencrypt = isencrypt;
	}
}
