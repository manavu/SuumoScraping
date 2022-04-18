# SuumoScraping

## 環境構築

- WSL をインストールする。事前の「仮想マシンプラットフォーム」のインストールを忘れずに！
- WSL から下記のコマンドを実行し、sshキーをコンテナに渡せるようにする
```
$ sudo apt-get install keychain
$ /usr/bin/keychain -q --nogui $HOME/.ssh/id_rsa # ここでSSHの秘密鍵を指定する
$ source $HOME/.keychain/$NAME-sh
```
- VS Code をインストールする。
- 拡張機能で Remote - Containers をインストールする。
- ダウンロードしたプロジェクトのフォルダを開く。
- 左下にある>< アイコンをクリックし、Open Folder IN Container をクリックし、該当フォルダを選択する
- アプリケーションの実行時にはシークレットを使って接続文字列を渡しているので、下記のコマンドでシークレットを追加します。
```
$ dotnet user-secrets init --id "bde44560-6d21-40eb-bd09-82c35fa5c7cf"
$ dotnet user-secrets set "ConnectionStrings:ScrapingDb" "server=localhost;database=ScrapingDb;port=3306;uid=****;pwd=****;characterset=utf8;"
```

## 実行するには
```
$ dotnet run
$ curl -k -L -m 0  https://localhost:5001/Bukken/Import
```

## モデルに変更を加えたら

ef ツールをインストールする
```
$ dotnet tool restore
```

コードがコンパイルされ追加された変更点のみのマイグレーションファイルを生成する
```
$ dotnet dotnet-ef migrations add 追加するマイグレーションファイル名
```

追加されたマイグレーションファイルを実行する
```
$ dotnet dotnet-ef database update
```

## datetime(6) になる

モデルから DDL を生成すると datetime(6) で型が定義されてしまう。
もともとは datetime だけだったが、なぜかこのような形になってしまう。
内部的に型が違うので不整合が起きる

## idb から復元する方法

frm ファイルが壊れてしまった場合、次の方法で復元が可能

idb ファイルを安全な場所に移動させる。またはコピーする
idb ファイルを削除し、テーブルを再度作成する。同じスキーマであることが重要
mysql へログインし下記の手順を実行する
mysql> SET FOREIGN_KEY_CHECKS = 0;
mysql> LOCK TABLES テーブル名 WRITE;
mysql> ALTER TABLE テーブル名 DISCARD TABLESPACE;
バックアップを取った idb ファイルを、データベースのフォルダにコピーする
mysql> ALTER TABLE テーブル名 IMPORT TABLESPACE;
mysql> UNLOCK TABLES;