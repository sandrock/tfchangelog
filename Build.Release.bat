
@echo off

echo:
echo TfChangelog build script (Release)
echo:

set bb.build.msbuild.exe=
for /D %%D in (%SYSTEMROOT%\Microsoft.NET\Framework\v4*) do set msbuild.exe=%%D\MSBuild.exe

echo Building TfChangelog...
%msbuild.exe% /p:Configuration=Release /v:quiet TfChangelog.sln
if not %ERRORLEVEL% == 0 (
  echo Oops. Something went wrong. Do you have .NET Framework 4.0 installed?
  goto end
)


echo Done.
echo Binaries located in TfChangelog\bin\Release
start TfChangelog\bin\Release

:end
