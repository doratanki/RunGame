#!/bin/bash
# Stack Tower プロジェクトを Unity 6000.4.0f1 で開く

UNITY="/Applications/Unity/Hub/Editor/6000.4.0f1/Unity.app/Contents/MacOS/Unity"
PROJECT="$(cd "$(dirname "$0")"; pwd)"

echo "Unity を起動中..."
echo "プロジェクト: $PROJECT"

open -a "$UNITY" --args -projectPath "$PROJECT"
