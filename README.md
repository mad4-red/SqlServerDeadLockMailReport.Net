# SqlServerDeadLockMailReport.Net
拡張イベントの `ring_buffer` からデッドロックグラフを抽出してメール送信するバッチ

## 拡張イベント
既定の `system_health` でもいいけれど、デッドロックだけ拾う拡張イベントを作ったほうが便利

## タスク実行
Windows のタスクスケジューラーで定期的に叩いて下さい

## なんで作ったか
デッドロックが起きた時に出来るだけ早く気付きたいけれど、わざわざSSMSを立ち上げてサーバーに繋いで拡張イベントを見るのが面倒だったので  
メール通知しつつデッドロックグラフも添付したかったから

## 開発環境・言語
C# .NET Framework 4.5.2  
Visual Studio 2013 で開発しています

## ライセンス
[The MIT License (MIT)](https://raw.githubusercontent.com/mad4-red/SqlServerDeadLockMailReport.Net/master/LICENSE)

MIT ライセンスの下で公開するオープンソースソフトウェアです