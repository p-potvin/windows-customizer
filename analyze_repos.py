import requests
import json
import os
import re

GITHUB_USER = "p-potvin"

def get_repos(username):
    repos = []
    page = 1
    while True:
        # We don't have token but it's public data
        url = f"https://api.github.com/users/{username}/repos?per_page=100&page={page}"
        response = requests.get(url)
        if response.status_code != 200:
            break
        data = response.json()
        if not data:
            break
        repos.extend(data)
        page += 1
    return repos

# Let's read the list we calculated earlier
try:
    with open('/tmp/repos_percentages.txt', 'r') as f:
        percentages = dict(line.strip().split(':') for line in f if ':' in line)
except FileNotFoundError:
    percentages = {}

if __name__ == "__main__":
    if not percentages:
        print("Please run the bash analysis first to calculate percentages.")
        exit(1)

    print(f"Analysis of all {len(percentages)} projects for {GITHUB_USER}:\n")
    for repo, pct in sorted(percentages.items()):
        print(f"- {repo}: {pct}%")
