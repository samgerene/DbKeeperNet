rem if "%1"  "" goto startBuild
echo Enter a target from build script (SrcZip,Build,BinZip)

:startBuild
C:\WINDOWS\Microsoft.NET\Framework\v3.5\msbuild.exe DbKeeperNet.xml /Target:%1 /l:FileLogger,Microsoft.Build.Engine;logfile=DbKeeperNetBuild.log

:end