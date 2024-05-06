#!/bin/bash

# Vars
slndir="$(dirname "${BASH_SOURCE[0]}")/"

# Restore + Build
dotnet build "$slndir/CompilerCSharp" --nologo || exit

# Run
dotnet run -p "$slndir/CompilerCSharp" --no-build -- "$@"