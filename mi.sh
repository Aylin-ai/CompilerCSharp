#!/bin/bash

# Vars
slndir="$(dirname "${BASH_SOURCE[0]}")/"

# Restore + Build
dotnet build "$slndir/InterpreterCSharp" --nologo || exit

# Run
dotnet run -p "$slndir/InterpreterCSharp" --no-build