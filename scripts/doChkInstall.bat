@echo off

if exist "%USERPROFILE%\source/repos/netTTS/" (
   echo Found netTTS installation.
) else (
  echo Did not find netTTS installation.
)

if exist "%USERPROFILE%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\netTTSSrvStart.lnk" (
  echo Found netTTS shortcut in startup folder.
) else (
  echo Did not find netTTS shortcut in start up folder.
)

call %USERPROFILE%\source\repos\netTTS\scripts\doShow.bat

if exist "%USERPROFILE%\source/repos/netTTS/netTTS.log" (
   echo Found netTTS log file.
) else (
    echo Did not find netTTS log file.
)

rem ends here.
