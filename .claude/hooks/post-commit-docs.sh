#!/bin/bash

# Post-commit hook to remind about documentation updates
# Runs after git commands complete in Claude Code sessions

set -e

# Read the incoming JSON data
input=$(cat)

# Extract the bash command that was executed
command=$(echo "$input" | jq -r '.tool_input.command // empty' 2>/dev/null || echo "")

# Only trigger on git commit commands (not git status, git diff, etc.)
if [[ ! "$command" =~ git[[:space:]]commit ]]; then
  exit 0
fi

# Output context to remind about documentation
cat <<'EOF'
{
  "hookSpecificOutput": {
    "hookEventName": "PostToolUse",
    "additionalContext": "Git commit completed. Consider running the post-implementation-docs skill to update documentation (README, ADRs, API specs) based on the changes."
  }
}
EOF

exit 0
