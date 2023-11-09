@echo off

set /p TPID=<netTTS.pid
echo Killing process %TPID%
taskkill /f /pid %TPID%
del netTTS.pid

rem Ends here.
