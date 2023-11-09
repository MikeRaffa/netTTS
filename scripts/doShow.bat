@echo off
rem NB: This SET command does not seem to work unless it's at top level.
set /p TPID=<"%USERPROFILE%\source\repos\netTTS\netTTS.pid"
if exist "%USERPROFILE%\source\repos\netTTS\netTTS.pid" (
   echo Found netTTS PID file.  TASKLIST  output filtered on PID %TPID% is:
   tasklist /fi "pid eq %TPID%" /nh
) else (
   echo Did not find netTTS PID file.
   echo TASKLIST  output filtered on IMAGENAME netTTS is:
   tasklist /fi "imagename eq netTTS.exe" /nh
)

rem Ends here.

