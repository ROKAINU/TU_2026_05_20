| Level   | 意味              | ゲーム継続 |
| ------- |   ---------  　　 |   -----   |
| Debug   | 開発用詳細情報     | 継続 　　　|
| Info    | 正常イベント   　　| 継続   　  |
| Warning | 問題あるが継続可能 | 継続       |
| Error   | 処理失敗          | 一部失敗　  |
| Fatal   | 致命障害          | 継続困難　　|

Debug   : 開発者しか興味ない内部状態についてのログ。リリース版では消しても困らないものに使う。
info    : 正常な重要イベントについてのログ。運営ログとして残したいものに使う。
Warning : 問題はあるがまだ動く状態で用いるログ。本当に見てほしい問題に使う。
Error   : 処理が失敗した時に用いるログ。期待していた処理が成立しなかった際に使う。
Fatal   : コードバグや通信失敗などの際に用いるログ。もう継続できない場合やゲーム続行が不可能な場合にも使う。

Unityゲームでの具体例
Debug   : Enemy State Changed
Info    : Scene Loaded
Warning : Save slot missing. Creating new one.
Error   : Save failed
Fatal   : MasterData load failed