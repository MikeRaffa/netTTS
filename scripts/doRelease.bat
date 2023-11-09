@echo off

call scripts\\build_env.cmd

%MSB% netTTS.sln /t:Rebuild /p:Configuration=Release

rem Ends here
