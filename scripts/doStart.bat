rem @echo off

echo "DLDdd doStart.bat"
dir
if exist netTTS\bin\Release\netTTS.exe (
    echo "Starting Release version of netTTS on port %1%"
    .\netTTS\bin\Release\netTTS.exe %1%
) else (
    echo "Starting Debug version of netTTS on port %1%"
    .\netTTS\bin\Debug\netTTS.exe %1%
)

rem Ends here.

