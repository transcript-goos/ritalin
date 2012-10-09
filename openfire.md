Openfireの設定について(for Mac)
------------------------------

* 使うのやめたけど、せっかく書いたので残しておきます

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