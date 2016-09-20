makecert -a sha512 -r -n "CN=192.168.4.182" -b 01/01/2014 -e 12/31/2999 -sv UMP.Certificate.pvk UMP.Certificate.cer
cert2spc UMP.Certificate.cer UMP.Certificate.spc
pvk2pfx -pvk UMP.Certificate.pvk -spc UMP.Certificate.spc -pfx UMP.S.192.168.4.182.pfx -pi VoiceCyber,PF,123 -po VoiceCyber,123