#!/bin/bash

# 設定環境變數為生產環境
export ENVIRONMENT=Production

# 停止並刪除所有現有容器
docker stop $(docker ps -aq) 2>/dev/null || true
docker rm $(docker ps -aq) 2>/dev/null || true

# 創建 Docker 網絡
docker network create app-network 2>/dev/null || true

# 啟動 RabbitMQ 容器
echo "啟動 RabbitMQ 容器..."
docker run -d --name scheduleexample-rabbitmq \
  --network app-network \
  -p 5672:5672 \
  -p 15672:15672 \
  -v rabbitmq-data:/var/lib/rabbitmq \
  rabbitmq:3-management

# 啟動一個空的 Redis 替代容器
echo "啟動生產環境的空 Redis 容器..."
docker run -d --name scheduleexample-redis \
  --network app-network \
  alpine:latest \
  echo "Redis 服務在生產環境中不啟動"

# 等待 RabbitMQ 啟動
echo "等待 RabbitMQ 啟動 (10秒)..."
sleep 10

# 啟動 API 容器
echo "啟動 API 容器..."
docker run -d --name scheduleexample-api \
  --network app-network \
  -p 5566:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__Redis=scheduleexample-redis:6379 \
  -e ConnectionStrings__RabbitMQ=scheduleexample-rabbitmq:5672 \
  scheduleexample-api

echo "生產環境已啟動，網站可在 http://localhost:5566 訪問"
echo "RabbitMQ 管理界面可在 http://localhost:15672 訪問 (用戶名: guest, 密碼: guest)"

# 顯示容器狀態
docker ps

# 顯示 API 日誌
sleep 2  # 等待容器完全啟動
docker logs scheduleexample-api
