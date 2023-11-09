@echo off

call %USERPROFILE%\source\repos\netTTS\scripts\doStop.bat
call %USERPROFILE%\source\repos\netTTS\scripts\rmShortcut.bat
if exist ".\scripts" (
   rem    ren netTTS netTTS_PREVIOUS
   echo Left you in netTTS install directory, you might want to rename or delete this to avoid confusion, just make sure you haven't changed anything you can't get back.
) else (
  echo "Left your installation directory in source/repos/netTTS, you might want to rename or delete that to avoid confusion, just make sure you haven't changed anything you can't get back.
)


rem Ends here.


