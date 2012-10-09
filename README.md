公開処刑：実践テスト駆動開発の写経
================================

* PArt IIIのAuctionSniperを写経
* C#で
* Mac上のMonoなので、とりあえずGUIは避けて

ライセンス
-----------
このリポジトリに含まれているものはApache2.0予定らしいよ

XMPPサーバーの設定について
------------------------

* 当初、[Openfire](oprnfire.md)でいこうと思ってましたが、DBに保存できなかったり、クライアントライブラリからつなげなかったり（やり方悪かったかも）と、
散々な目にあったので、Erlangで書かれた、ejabberdに鞍替え。
* インストーラー使えば、指示に従うだけで、特別な設定は不要
* せいぜい、規定言語をjaにしたくらい
* ただし、Mac版だと、launchd用のplistないので立ち上げは毎回自分でとなる（書けばいいんだけども）。

Openfireの設定について(for Mac)
----------------------

* データベース(SQLiteの場合)
    * jdbc driver
        * http://www.xerial.org/trac/Xerial/wiki/SQLiteJDBC
        * openfire/libに放り込む
    * データベース作る
        * sudo sqlite3 /usr/local/openfire/embedded-db/openfire.sqlite3
    * テーブル作る
        * openfire/resource/database/openfire_db2.sqlがそのまま使えた

* 実行方法
    * sudo launchctl load /Library/LaunchDaemons/org.jivesoftware.openfire.plist
* 停止方法
    * sudo launchctl unload /Library/LaunchDaemons/org.jivesoftware.openfire.plist

* 初期設定
    1. Openfireを実行する
    2. localhost:9090にアクセス
    3. 設定項目埋めていく
        * JDBC
            * class -> org.sqlite.JDBC
            * url -> jdbc:sqlite:openfire.sqlite3
        * 中途半端にopenfire/confopenfire.xmlが作られるとぬるポになるので注意
        * 削除すればおk