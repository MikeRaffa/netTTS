@echo off

call scripts\\build_env.cmd

%MSB% netTTS.sln /t:Clean
%MSB% netTTS.sln /t:Clean /p:Configuration=Release

rem ends here.
