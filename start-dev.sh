#!/bin/bash

# 設定環境變數為開發環境
export ENVIRONMENT=Development

# 啟動 Docker Compose 服務，使用開發環境配置文件
docker compose --profile dev up -d

echo "開發環境已啟動，網站可在 http://localhost:5566 訪問"
echo "Redis 管理界面可在 http://localhost:6379 訪問"
