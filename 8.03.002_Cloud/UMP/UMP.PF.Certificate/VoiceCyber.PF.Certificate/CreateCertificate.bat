makecert -a sha512 -r -n "CN=VoiceCyber.PF" -b 01/01/2014 -e 12/31/2999 -sv VoiceCyber.PF.Certificate.pvk VoiceCyber.PF.Certificate.cer
cert2spc VoiceCyber.PF.Certificate.cer VoiceCyber.PF.Certificate.spc
pvk2pfx -pvk VoiceCyber.PF.Certificate.pvk -spc VoiceCyber.PF.Certificate.spc -pfx VoiceCyber.PF.Certificate.pfx -pi VoiceCyber,PF,123 -po VoiceCyber,123