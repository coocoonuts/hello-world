# CatFacts
Overview
  Windows service to query web api for a random cat fact. Convert the fact from text to speach. Basic audio controls to ensure the person hears it.

Config:
  Volume to do the text to speach at
  Range in which to randomly wait between facts
  A greeting for the listener
  
To Install:
  Downlaod release folder
  run installutil <Path to exe>
    If this fails try unblocking the exe file first (security)
  
To Run:
  Just start the service
  It doesn't start the whole way and gets stuck in a status of "starting"
  
To Stop/Uninstall
  Find the PID and kill it
  Run installutil /u <Path to exe>
