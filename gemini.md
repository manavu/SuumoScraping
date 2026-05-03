# SuumoScraping - プロジェクトコンテキスト

## 概要
このプロジェクトは、不動産情報サイト（Suumo）から物件情報をスクレイピング・インポートし、管理するための ASP.NET Core MVC アプリケーションです。
開発環境は Docker (Dev Containers) を前提としており、データベースには MySQL を使用しています。

## 技術スタック
- **フレームワーク:** .NET 10.0 (ASP.NET Core MVC)
- **言語:** C# 14
- **データベース:** MySQL 8.4 (LTS)
- **ORM:** Entity Framework Core 10.0 (MySql.EntityFrameworkCore)
- **スクレイピング:** HtmlAgilityPack
- **開発環境:** Docker, VS Code Remote - Containers

## 環境構築
1. **前提:** WSL2 および Docker Desktop がインストールされていること。
2. **起動:** VS Code の "Remote - Containers: Reopen in Container" を使用して開発コンテナを起動します。
3. **設定:** `dotnet user-secrets` を使用して DB 接続文字列を設定します（README.md 参照）。

## ディレクトリ構成
- **Controllers/**: リクエストを処理するコントローラー (`BukkenController` 等)
- **Models/**: データベースエンティティおよびビジネスロジック
- **ViewModels/**: ビュー表示用のデータモデル
- **Views/**: Razor View (.cshtml) ファイル
- **Extensions/**: 拡張メソッド群
- **Migrations/**: Entity Framework Core のマイグレーションファイル
- **.devcontainer/**: Docker 開発環境の設定 (`docker-compose.yml`, `Dockerfile`)

## 主要コマンド
### アプリケーション実行
```bash
dotnet run
```
実行後、`http://localhost:5000` でアクセス可能。

### CLI コマンド (スクレイピング・同期)
スクレイピングの実行:
```bash
dotnet run -- scrape
```
データの同期 (集計):
```bash
dotnet run -- sync
```
※ いずれも `Ctrl+C` で安全に中断可能です。

### データベース連携 (Entity Framework Core)
ツールの復元:
```bash
dotnet tool restore
```
マイグレーションの作成:
```bash
dotnet dotnet-ef migrations add [MigrationName]
```
データベースの更新:
```bash
dotnet dotnet-ef database update
```

## 注意点
- **MySQL設定:** `docker-compose.yml` で定義。ポート 3306 で公開。
- **日付型:** MySQL の `DATETIME(6)` 問題（README 参照）に注意が必要。
- **シークレット:** 接続情報はコミットせず、User Secrets で管理すること。
