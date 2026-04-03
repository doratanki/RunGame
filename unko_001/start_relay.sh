#!/bin/bash
# Unity MCP relay を起動するラッパースクリプト
# Claude Code の MCP 設定から直接呼ばれる

PROJECT_DIR="$(cd "$(dirname "$0")"; pwd)"

# 最新の com.unity.ai.assistant パスを動的に検索
RELAY=$(find "$PROJECT_DIR/Library/PackageCache" -name "relay_mac_arm64" -not -name "*.app" 2>/dev/null | head -1)

if [ -z "$RELAY" ]; then
    echo "relay が見つかりません" >&2
    exit 1
fi

chmod +x "$RELAY"
exec "$RELAY" --mcp
