@echo off

cscript %USERPROFILE%\source\repos\netTTS\scripts\mkShortcut.vbs

if exist "%USERPROFILE%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\netTTSSrvStart.lnk" (
   echo The netTTS service is now set up to automatically start when you log in.
   echo Please log out and back in to start the service.  Thank You!
) else (
  echo Sorry, but something has gone wrong, please report a bug or check out the files under the scripts directory to take a look yourself.
  exit /b 13
)

rem Ends here.
