 
mshta VBScript:Execute("Set a=CreateObject(""WScript.Shell""):Set b=a.CreateShortcut(a.SpecialFolders(""Startup"") & ""\SF.lnk""):b.TargetPath=""%~dp0SFDemo.exe"":b.WorkingDirectory=""%~dp0"":b.Save:close")
 mshta VBScript:Execute("Set a=CreateObject(""WScript.Shell""):Set b=a.CreateShortcut(a.SpecialFolders(""Desktop"") & ""\SF.lnk""):b.TargetPath=""%~dp0SFDemo.exe"":b.WorkingDirectory=""%~dp0"":b.Save:close")

@pause