#!/bin/bash
# Script to help the user update all their repos with the percentages
# Since we only have push access to the current repo within this agent session,
# we provide this script so the user can run it locally with their git credentials.

if [ ! -f "repos_percentages.txt" ]; then
    echo "Creating repos_percentages.txt from previous analysis..."
    cat << 'INNER_EOF' > repos_percentages.txt
Video-Depth-Anything:10
agentic-markup:100
auto-backup:80
automation-suite:100
cultural-rhythm:10
debrid-media-manager:6
vaultwares-decompile:100
vaultwares-dispatch:10
event-horizon:100
facefusion:10
firebase-apphosting:10
vaultwares-glass:10
nemo-playground:33
no-more-groceries:50
vaultwares-realtime:100
traffic-pulse:100
vaultwares-studio:100
vault-central:100
vault-explorer:100
vault-flows:100
vault-player:56
vault-themes:33
vaultwares-agentciation:50
vaultwares-cli:75
vaultwares-docs:100
vaultwares-identity-manager:14
vaultwares-pipelines:100
vaultwares-template:0
vaultwares-website:100
video-transcriber-translator:100
weekly-menu:11
windows-customizer:85
INNER_EOF
fi

while IFS=':' read -r repo_name pct; do
    echo "Updating $repo_name with $pct%..."
    if [ ! -d "$repo_name" ]; then
        git clone "https://github.com/p-potvin/$repo_name.git"
    fi
    if [ -d "$repo_name" ]; then
        cd "$repo_name"
        mkdir -p .github
        echo "$pct" > .github/project_percentage.txt
        git add .github/project_percentage.txt
        git commit -m "chore: add project completion percentage ($pct%)"
        # NOTE: git push removed to prevent blocking the session. Please run git push manually for each repository.
        cd ..
    fi
done < repos_percentages.txt
