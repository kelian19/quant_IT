#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
# Existing checkout is already isolated; no worktree needed.
dotnet build --no-restore
dotnet run --no-build --project src/EquityQuant.Api --urls http://127.0.0.1:5080 &
api_pid=$!
trap 'kill "$api_pid" 2>/dev/null || true' EXIT INT TERM
dotnet run --no-build --project src/EquityQuant.Web --urls http://127.0.0.1:5081
