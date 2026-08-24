#!/bin/bash

# 使用方法を表示する関数
function show_usage() {
    echo "使用方法:"
    echo "  ./run.sh web       - Webサーバーを起動します (http://localhost:5000)"
    echo "  ./run.sh scrape    - スクレイピングを実行します"
    echo "  ./run.sh sync      - スクレイピングデータの集計・同期を実行します"
    echo "  ./run.sh db-update - データベースのマイグレーションを適用します"
    echo "  ./run.sh help      - このヘルプメッセージを表示します"
}

# 引数の確認
if [ $# -eq 0 ]; then
    show_usage
    exit 1
fi

case "$1" in
    web)
        echo "Webサーバーを起動しています..."
        dotnet run --project SuumoScraping.Web
        ;;
    scrape)
        echo "スクレイピングを実行しています..."
        dotnet run --project SuumoScraping.Web -- scrape
        ;;
    sync)
        echo "データの同期（集計）を実行しています..."
        dotnet run --project SuumoScraping.Web -- sync
        ;;
    db-update)
        echo "データベースのマイグレーションを適用しています..."
        dotnet ef database update --project SuumoScraping.Infrastructure --startup-project SuumoScraping.Web
        ;;
    help|--help|-h)
        show_usage
        ;;
    *)
        echo "エラー: 未知のコマンド '$1'"
        show_usage
        exit 1
        ;;
esac
