This program uses the N1MM Logger+ Radio UDP broadcast to send commands to an IP-controlled Antenna Genius.
Syntax: N1MM_to_AG.exe [IP address of AG] (Note: this is a Windows Form application)

Considerations:
- N1MM_to_AG.exe listens on port 13090. Change the code if you want to listen on a different port number.
- In N1MM Logger+, in Configurer->Broadcast Data, the Radio box should be checked, and the IP address:port added. Example: 127.0.0.1:13090
- This program assumes that the AG Band Configuration default values are used.
- The Antenna Genius program does not need to be running, but may be useful to see what antenna has been selected.