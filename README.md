# SuumoScraping

## 環境構築

VS Code をインストールします。

拡張機能で Remote - Containers をインストールする。

ダウンロードしたプロジェクトのフォルダを開く。

左下にある>< アイコンをクリックし、Open Folder IN Container をクリックし、該当フォルダを選択する

$ curl -k -L -m 0  https://localhost:5001/Bukken/Import2

アプリケーションの実行時にはシークレットを使って接続文字列を渡しているので、下記のコマンドでシークレットを追加します。

```
dotnet user-secrets init --id "bde44560-6d21-40eb-bd09-82c35fa5c7cf"
dotnet user-secrets set "ConnectionStrings:ScrapingDb" "server=localhost;database=ScrapingDb;port=3306;uid=****;pwd=****;characterset=utf8;"
```
