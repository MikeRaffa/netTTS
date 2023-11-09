rem @echo off

cd %USERPROFILE%\source\repos\netTTS\scripts
>..\netTTS.log (
  echo "DLDdnetTTSSrvStart"
  dir
  cd ..
  dir
  echo "DLDdd calling doStart 6602"
  call scripts\doStart.bat 6602
  cd scripts
)

rem Ends here.

