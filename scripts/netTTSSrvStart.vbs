
Set oShell = CreateObject ("Wscript.Shell")
Dim strArgs
strArgs = "cmd /c scripts\netTTSSrvStart.bat"
oShell.Run strArgs, 0, false
