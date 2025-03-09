#!/bin/bash

# 設定環境變數為開發環境
export ENVIRONMENT=Development

# 停止並刪除所有現有容器
docker stop $(docker ps -aq) 2>/dev/null || true
docker rm $(docker ps -aq) 2>/dev/null || true

# 創建 Docker 網絡
docker network create app-network 2>/dev/null || true

# 啟動 Redis 容器
echo "啟動 Redis 容器..."
docker run -d --name scheduleexample-redis \
  --network app-network \
  -p 6379:6379 \
  -v redis-data:/data \
  redis:latest

# 啟動一個空的 RabbitMQ 替代容器
echo "啟動開發環境的空 RabbitMQ 容器..."
docker run -d --name scheduleexample-rabbitmq \
  --network app-network \
  alpine:latest \
  echo "RabbitMQ 服務在開發環境中不啟動"

# 啟動 API 容器
echo "啟動 API 容器..."
docker run -d --name scheduleexample-api \
  --network app-network \
  -p 5566:80 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__Redis=scheduleexample-redis:6379 \
  -e ConnectionStrings__RabbitMQ=scheduleexample-rabbitmq:5672 \
  scheduleexample-api

echo "開發環境已啟動，網站可在 http://localhost:5566 訪問"
echo "Redis 管理界面可在 http://localhost:6379 訪問"

# 顯示容器狀態
docker ps

# 顯示 API 日誌
sleep 2  # 等待容器完全啟動
docker logs scheduleexample-api
