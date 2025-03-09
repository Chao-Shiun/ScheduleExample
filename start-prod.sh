#!/bin/bash

# 設定環境變數為生產環境
export ENVIRONMENT=Production

# 啟動 Docker Compose 服務，使用生產環境配置文件
docker compose --profile prod up -d

echo "生產環境已啟動，網站可在 http://localhost:5566 訪問"
echo "Redis 管理界面可在 http://localhost:6379 訪問"
echo "RabbitMQ 管理界面可在 http://localhost:15672 訪問 (用戶名: guest, 密碼: guest)"
