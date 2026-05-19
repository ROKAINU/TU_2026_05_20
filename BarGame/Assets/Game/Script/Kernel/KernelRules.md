# なぜ Kernel を Abstract / Pure / Events / Utils に分けるのか

「Unityへの依存があるかどうか」と「役割の種類」の2軸で分けるため。

Kernel はどの層でも使う汎用部品の置き場だが、全部を Kernel/ に平置きにすると「この型はテストで使えるの？」「Unityが必要なの？」が一目でわからなくなる

PureとAbstractのinterfaceのみDomainの依存が許可される


# 各サブフォルダの意味と分類基準

Kernel/
├── Abstract/   ← 契約・インターフェイス
├── Pure/       ← 概念・ロジック(Unity非依存の純粋C#型)
├── Events/     ← Pub/Sub・通信機構の仕組み（R3依存）
└── Utils/      ← その他汎用道具
    └── Unity/  ← Unity依存

## Abstract/ — 「何ができるか」の契約だけ書く場所
- 入れるもの	IRandom, ITime, IInputService などのインターフェイス
- Unity依存	なし（interface だけなので）
- 役割	Domain/Application が「実装を知らずに使える」ようにする

Abstract/ にあることで → 「これはDIで差し替えられる契約だ」と一目でわかる

IRandom   → テスト時は FixedRandom、本番は UnityRandomImpl
ITime     → テスト時は MockTime、本番は UnityTimeImpl

分類基準： interface または abstract class のみ。実装コードが一行も入ってはいけない。


## Pure/ — 「Unityなしで動く」ことを保証する場所
- 入れるもの	Result<T>, Option<T>, ValueObject基底など
- Unity依存	絶対にない（UnityEngine を using しない）
- 役割	ドメインテストをUnityなしの純粋C#環境で実行できる

Pure/ にあることで → 「このファイルはどこでもビルドできる」という保証になる

Result<T>  → .NET Console でもテストできる
Option<T>  → Unity Editor なしの CI でも動く

分類基準： using UnityEngine; が 一行もないこと。
これが壊れたら Pure に置いてはいけない。


## Events/ — 「層をまたぐ通知の仕組み」を置く場所
- 入れるもの	EventBus, IEvent マーカーなど
- Unity依存	R3 依存あり（Unity依存ではないがライブラリ依存）
- 役割	Domain → Application → Presentation の一方向通知を疎結合にする
- 特徴
    ・状態を持たない通信レール
    ・同期/非同期を抽象化する
    ・Reactiveライブラリに依存することがある

分類基準：
    「イベント駆動の通信を実現するための仕組みか？」

Events/ にあることで → 「ここを見ればPub/Subの全体像がわかる」

EventBus  → どの層も直接参照できる共有チャンネル
IEvent    → イベント型の基底マーカー

Pure に入れない理由： R3（Observable）への依存があるので Pure の「完全Unity/ライブラリフリー」保証が守れない。
Utils に入れない理由： イベントシステムは単なる道具ではなく、アーキテクチャの通信レールという独立した関心事。


## Utils/ — 「上の3つに分類できない汎用道具」
- 入れるもの	DisposableBag, 拡張メソッド, 小物ヘルパー
- Unity依存	ものによる（混在してよい）
- 役割	分類しきれないが複数箇所で使う小物のゴミ箱（良い意味で）

分類基準： Abstract・Pure・Events のどれにも当てはまらず、複数の層で使う汎用コードであること。