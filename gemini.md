# SuumoScraping - プロジェクトコンテキスト（エージェントガイド）

## 概要
このプロジェクトは、不動産情報サイト（Suumo）から物件情報をスクレイピング・インポートし、管理するための ASP.NET Core MVC アプリケーションです。
開発環境は Docker (Dev Containers) を前提としており、データベースには MySQL を使用しています。

元々モノリシックで密結合だったコードを、ドメイン駆動設計（DDD）に基づいてリファクタリングし、クリーンなレイヤー設計、完全な非同期化（`async/await`）、および徹底した静的解析とフォーマット標準化（StyleCop & CSharpier）を行っています。

---

## 技術スタックとコード品質ルール
- **フレームワーク:** .NET 10.0 (ASP.NET Core MVC)
- **言語:** C# 14
- **データベース:** MySQL 8.4 (LTS)
- **ORM:** Entity Framework Core 10.0 (MySql.EntityFrameworkCore)
- **スクレイピング:** HtmlAgilityPack
- **開発環境:** Docker (Dev Containers)
- **静的コード解析:** `StyleCop.Analyzers` (警告 0 件を維持)
- **コードフォーマッタ:** `CSharpier` (Prettierベースの自動整形を導入)
- **ロギング:** 標準の `ILogger` による構造化ログの全面採用
- **非同期設計:** HttpClient の通信からユースケース、コントローラーの戻り値まで `async/await` で統一

---

## レイヤーアーキテクチャ（ディレクトリ構成）
プロジェクトは以下のレイヤー構造に分離され、関心の分離（Separation of Concerns）を徹底しています。

- **`Domain/` (ドメイン層)**: 外部技術（HTTP/DB）に依存しないビジネスコア。
  - `Domain/Models/`: 物件詳細などの型安全なドメインモデル（DTO）。
  - `Domain/Gateways/`: 通信やパース、データアクセスなど、インフラ層に実装させる抽象インターフェース定義 (`ISuumoGateway`, `ISuumoHtmlFetcher`, `ISuumoHtmlParser` 等)。
  - `Domain/Exceptions/`: システム固有のカスタム例外定義 (`SuumoExceptions.cs`)。
- **`UseCases/` (ユースケース/アプリケーションサービス層)**: ビジネスシナリオの調整。
  - 各ビジネス要件が個別のクラス（例: `GetFilteredBukkensUseCase`, `SyncBukkensUseCase` 等) としてカプセル化されています。
- **`Infrastructure/` (インフラストラクチャ層)**: 外部技術の詳細実装。
  - `Infrastructure/Scraping/`: `HttpClient` による I/O を担う `SuumoHtmlFetcher` や、`HtmlAgilityPack` に依存した `SuumoHtmlParser`、これらを協調させる `SuumoGateway` が配置されています。
- **`Controllers/` (プレゼンテーション層)**: MVC コントローラー。
  - 直接データベースコンテキストや外部通信に依存せず、`UseCases` を注入（DI）して実行し、ビュー（`Views/`）にデータをマッピングします。
- **`Models/` (データアクセスエンティティ)**:
  - EF Core の DB エンティティ定義、および DB コンテキスト (`ScrapingContext.cs`)。

---

## 主要コマンド（AIエージェント向け）
エージェントは、すべての検証や実行コマンドを **Docker 開発コンテナ内** で実行する必要があります。
`docker` で始まるコマンドは、**ユーザーへの承認プロンプトを挟むことなく自動で直接実行可能**になっています。

### コンテナ内でのビルド
```bash
docker exec -t suumoscraping_devcontainer-app-1 dotnet build workspace.sln
```
※ `StyleCop.Analyzers` の静的解析が走ります。警告 0 件（NU1903の脆弱性警告を除く）でのビルド成功を維持してください。

### コンテナ内でのテスト実行
```bash
docker exec -t suumoscraping_devcontainer-app-1 dotnet test workspace.sln
```
※ すべてのテスト（3件）が 100% パスすることを確認してください。

### CSharpier によるコード自動整形
変更を加えた後は、必ず以下のコマンドを実行してコードスタイルを整形してください。
```bash
docker exec -t suumoscraping_devcontainer-app-1 dotnet csharpier format .
```
※ CSharpier のルールに適合させることで、StyleCop のフォーマット競合警告を自動的に回避できます。

### CLI コマンド (スクレイピング・同期)
開発コンテナ内で以下のコマンドを使用してバッチ処理を実行できます。
- スクレイピングの実行:
  ```bash
  docker exec -t suumoscraping_devcontainer-app-1 dotnet run -- scrape
  ```
- データの同期 (集計):
  ```bash
  docker exec -t suumoscraping_devcontainer-app-1 dotnet run -- sync
  ```
  ※ `Ctrl+C` で安全に中断可能です。

---

## エージェントの作業時の注意点
1. **ビジネスルールの維持**: 物件重複排除、価格履歴作成、築年数のパースといったコアルールは絶対に壊さないでください。
2. **完全非同期処理の徹底**: 新たにI/O（ネットワーク、ファイル、DB等）を伴う処理を追加する際は、必ず `async/await` を用いて非同期で記述してください（`.Result` や `.Wait()` による同期ブロッキングは厳禁）。
3. **ロギングとエラーハンドリング**: 汎用的な `Console.WriteLine` などは避け、`ILogger` を注入して構造化ログを出力し、適切なカスタム例外 (`SuumoScrapingException` 等) をスローしてください。
