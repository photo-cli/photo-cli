#!/usr/bin/env bash
# Syncs AGENTS.md (canonical source) to the file locations expected by each agentic tool.
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SRC="$REPO_ROOT/AGENTS.md"

sync() {
	local dest="$REPO_ROOT/$1"
	mkdir -p "$(dirname "$dest")"
	cp "$SRC" "$dest"
	echo "synced: $1"
}

sync "CLAUDE.md"
sync ".github/copilot-instructions.md"
sync ".cursor/rules/rules.mdc"
sync ".windsurfrules"
