# SuumoScraping

## 環境構築

Visual Studio 2019 以降をインストールします。

.net core sdk 3.1 をインストールします。

DBにMySql5.7 以降をインストールして必要最低限の環境を構築しときます。

Visual Studio を起動し、ソリューションをビルドします。

パッケージマネージャコンソールを開きます。
下記のコマンドラインのConnectionString の引数を適切な値に変更してパッケージマネージャコンソールから実行します。

```
update-database -verbose -ConnectionProviderName "MySql.Data.MySqlClient" -ConnectionString "server=localhost;database=ScrapingDb;port=3306;characterset=utf8;uid=****;pwd=****;"
```

アプリケーションの実行時にはシークレットを使って接続文字列を渡しているので、下記のコマンドでシークレットを追加します。

```
dotnet user-secrets init --id "bde44560-6d21-40eb-bd09-82c35fa5c7cf"
dotnet user-secrets set "ConnectionStrings:ScrapingDb" "server=localhost;database=ScrapingDb;port=3306;uid=****;pwd=****;characterset=utf8;"
```
