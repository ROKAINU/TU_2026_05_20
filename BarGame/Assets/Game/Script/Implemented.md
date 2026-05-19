##### 実装したもの一覧

#### Application
### Save
## DebouncedSaver
1. SettingsDebouncedSaver
    設定変更が起きるたびに即保存せず、設定変更を一定間隔でまとめてセーブするデバウンス型永続化システム。

## SaveDataApplier
1. ISaveDataApplier
    ゲームのセーブデータをストアに適用する。セーブデータの内容に基づいて、ストアの状態を更新する。

#### Kernel
### Abstruct
1. Logger
    Logを抽象化したもの。

2. IRandom
    Randomを抽象化したもの

3. ITime
    Timeを抽象化したもの

### Pure
## Algorithm_Geometory
1. FillManhattanArea
    マンハッタン距離で指定された範囲を塗りつぶすアルゴリズム。
    中心からの距離が r 以下のセルを対象とする（菱形の範囲）。
    グリッドの境界を超えるセルは無視される。

2. LineAlgorithms
    Bresenham アルゴリズムを使用した直線アルゴリズム。
    Bresenham のアルゴリズムで2点間の直線上のセルを取得。

3. PathValidator
    パス検証ユーティリティ。
    グリッド内のパスが条件を満たしているか検証する。
    Unity非依存の汎用アルゴリズム。

## Geometory
1. Int2
    整数2次元ベクトル

    var a = new Int2(1, 2);
    var b = new Int2(4, 6);
    - マンハッタン距離
        int dist = a.ManhattanDistance(b); // 7
    - チェビシェフ距離
        int dist = a.ChebyshevDistance(b); // 4
    - 二乗ユークリッド距離
        long dist2 = a.SquaredEuclideanDistance(b); // 25
    - 原点からの距離（二乗）
        long mag2 = a.MagnitudeSquared; // 1² + 2² = 5

2. Coord2D
    浮動小数2次元ベクトル

## StringGeneraotr
1. GenerateRandomCode
    指定された文字列セットからランダムなコードを生成。

## Pure
1. ConvertVolume
    音量と0~1のfloatの変換ができる。

    人の聴覚特性に合わせた知覚的変換であるPerceptual,振幅比をそのままdBに変換するLinear,dBスケール上で線形にマッピングするAudioTaperの3方式を切り替えられる。

2. DisposableBag
    DisposableBag は、複数の IDisposable（購読解除・タイマー停止・ハンドル解放など）を「まとめて」後片付けするためのクラスです。

    Event購読や各種リソースは「解除し忘れ」でメモリリークや想定外動作になりやすい
    そこで「解除すべきものを袋に放り込む → 最後に袋を Dispose するだけ」にして、解除漏れを防ぎます

3. ObjectPool<T>
    高信頼・防御型の汎用オブジェクトプールで、オブジェクトを使い回して、生成コストをゼロに近づける目的がある
    オブジェクトの再利用・安全なライフサイクル管理・例外耐性・デバッグしやすさに重点を置いている

4. Option<T>
    null を使わずに「値がある／ない」を安全に表すため（None を型で表現する）。

5. Result<T>
    # 使用目的

    例外に頼らず「成功/失敗」を戻り値で表し、呼び出し側に失敗ハンドリングを強制したい。
    API の契約として「失敗する可能性がある」ことを型で表したい。

    ならResult、なら例外
    見分ける質問

    【Resultを用いるとき】
    - この失敗は“普通に起きるイベント”であるとき
    - その失敗を“分岐として扱う”時
    - ゲームロジックであるとき

    【例外処理を用いるとき】
    - “異常として止める”とき
    - 処理を中断するとき
    - バグ・壊れた状態であるとき

    # 使用方法

    Result<T>：成功なら T、失敗なら string エラー を持つ結果型
    Result<T,E>：成功なら T、失敗なら 任意のエラー型 E を持つ結果型

    # 動作

    IsOk / IsErr のフラグで状態を判定し
    成功時だけ Value を、失敗時だけ Error を参照できるようにする
    （間違って参照すると InvalidOperationException を投げてバグに気づける設計）

### Utils
## Abstruct
1. IAssetPreloader
    アセットの事前ロード（プリロード）と取得を担うインターフェース。

## InputCommandSystem_Cysharp
    コマンド・入力指示を安全に非同期で受け渡しするパイプライン。

    入力発生側                    処理側
    （誰かがコマンドを出す）  →  （誰かがコマンドを処理する）

    ICommandEmitter             IAsyncCommandQueue
        ↓                           ↓
    Enqueue()          →        NextAsync() で待って受け取る
                                    ↓
                                ICommandHandler
                                   ↓
                                HandleAsync()

1. AsyncCommandQueue<TCommand>
    非同期でコマンドを受け取るための待ち受け付きキュー

2. ICommandEmitter
    ゲーム内コマンドを外部に発行する装置、コマンド生成側（入力やイベント発火側）の抽象インターフェース。

3. ICommandHandler<TCommand>
    受け取ったコマンドを非同期で処理する役、コマンド実行側（処理系）の抽象インターフェース。

## R3
1. Store<T>
    - 状態を一箇所で持てる（Single Source of Truth）

    // 状態はここにしか存在しない
    var store = new Store<BattleState>(initialState);
    「BattleStateの現在値はStoreに聞けば必ずわかる」という保証。

    - 状態変更を Dispatch 経由に強制する
    
    // ✅ これしか状態を変える方法がない
    store.Dispatch(state => state with { PlayerHp = 50 });
    // ❌ 外から直接書き換えられない
    store.State.Value = ...; // ReadOnlyなので不可能
    
    誰かが勝手に状態を書き換える事を防ぐ。
    
    - 変化を購読・監視できる

## Unity
1. CoordinateConverter
    Kernel座標と Unity座標の相互変換ユーティリティ。
   
2. SpriteSheetSlicer
    スプライトシートのスライスユーティリティ。
    グリッドレイアウト形式のスプライトシートから個別スプライトを生成。
    
#### Test
1. TestMode
    TestScriptの実装

#### Editor
1. DevToolbarButtons
    処理をツールバーから行えるようにするもの
    セーブデータを消したりできる