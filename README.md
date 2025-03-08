# Schedule API 多環境配置

這個專案示範如何使用 Docker Compose 實現多環境部署，並通過依賴注入和開放封閉原則實現消息服務的無縫切換。

## 項目架構

- **ScheduleAPI**：.NET Core Web API 專案
- **消息服務**：支援 Redis 和 RabbitMQ 兩種實現
  - 測試環境：使用 Redis
  - 正式環境：使用 RabbitMQ

## 設計原則

1. **依賴注入 (DI)**：通過依賴注入容器註冊和解析消息服務
2. **開放封閉原則 (OCP)**：定義 `IMessageService` 接口，允許擴展新的實現而不修改客戶端代碼
3. **工廠模式**：使用 `MessageServiceFactory` 根據配置創建適當的消息服務實例

## 環境切換

### 配置文件
- `appsettings.json`：基本配置
- `appsettings.Development.json`：開發環境配置（Redis）
- `appsettings.Production.json`：生產環境配置（RabbitMQ）

### Docker Compose 文件
- `docker-compose.yml`：基本服務定義
- `docker-compose.development.yml`：開發環境配置（Redis）
- `docker-compose.production.yml`：生產環境配置（RabbitMQ）

## 啟動指令

### 開發環境（Redis）

```bash
# 設置環境變數
export ENVIRONMENT=Development
# 或在 Windows PowerShell 中
$env:ENVIRONMENT="Development"

# 使用開發環境配置啟動
docker-compose -f docker-compose.yml -f docker-compose.development.yml up -d
```

### 生產環境（RabbitMQ）

```bash
# 設置環境變數
export ENVIRONMENT=Production
# 或在 Windows PowerShell 中
$env:ENVIRONMENT="Production"

# 使用生產環境配置啟動
docker-compose -f docker-compose.yml -f docker-compose.production.yml up -d
```

## API 端點

- **POST /api/Order**：提交新訂單
  - 請求體範例：
    ```json
    {
      "OrderId": "ORD-001",
      "Amount": 100.50,
      "Customer": "測試客戶"
    }
    ```
  - 返回：訂單提交確認

## 擴展

如需添加新的消息服務實現：

1. 實現 `IMessageService` 接口
2. 在 `MessageServiceFactory` 中添加新的實現選擇
3. 更新配置文件以支持新的消息服務 