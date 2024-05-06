@echo off

REM Vars
set "SLNDIR=%~dp0"

REM Restore + Build
dotnet build "%SLNDIR%\CompilerCSharp" --nologo || exit /b

REM Run
dotnet run -p "%SLNDIR%\CompilerCSharp" --no-build -- %*
