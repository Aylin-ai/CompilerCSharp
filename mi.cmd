@echo off

REM Vars
set "SLNDIR=%~"

REM Restore + Build
dotnet build "%SLNDIR%\InterpreterCSharp" --nologo || exit /b

REM Run
dotnet run -p "%SLNDIR%\InterpreterCSharp" --no-build
