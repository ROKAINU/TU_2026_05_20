### ディレクトリ構造

基本的な判断基準
- それは何を知っている/触っているか
- 何が変わったら一緒に変更されるか

確認チェックリスト
 Domain に Unity型がないか？
 Domain が外部I/Oに依存していないか？
 Application が表示制御を決めていないか？
 Presentation がゲームルールを決めていないか？
 Infrastructure がビジネスロジックを含んでいないか？
 Kernel が特定のゲームルールに依存していないか？
 依存の向きが正しいか？（下位層が上位層に依存していないか）

## それぞれのディレクトリに入れるもの

0) **Domain**
# 原則A："純粋"であるほど価値が高い

Domainに入れるものは、できる限り
- Unity非依存
- 非同期ライブラリ非依存（UniTaskなど）
- I/O非依存
- テストしやすい（入力を渡せば出力が決まる）

Domainは一番汚してはいけない層

# 原則B：置き場は「依存の向き」で決める

Domain → どの層にも依存しない
Application → Domainに依存してよい
Infrastructure/Presentation → Application/Domainに依存してよい

# 入れるものの例

- ゲームルール（勝敗、移動可否、ダメージ計算、ターン進行規則）
- 状態遷移（Phase/Turn/EnemyStateの変化）
- 純粋なデータ（Unity型を含まないState/ValueObject）
- 乱数が必要でも「I RNG」を受け取るだけで、実装は知らない

「これはゲームの規則か？」
「Unityなしで動くか？」
「入力が与えられれば結果が決まるか？」

# 入れないものの例

- UniTask, CancellationToken を前提とした "待機"
- GameObject, AudioClip、Scene、TweenなどUnity副作用
- セーブ/ロード、ファイル、ネット、Addressables
- UIの表示都合

1) Application に入れる判断基準
# 入れるものの例

- ユースケース（プレイ開始、1ターン進める、リスタート要求処理）
- GameLoop / TurnSystem
- 入力→コマンド→ルール適用→結果生成の“進行制御”
- Domainの複数機能を束ねる調停（orchestration）

「いつ何を呼ぶかを決めているか？」
「複数のDomain要素を順番に実行しているか？」
「処理の流れを作っているか？」

# 入れないものの例

- 画面に何を表示するかの細部（Presentation）
- 具体的なUnity I/O実装（Infrastructure）
- 純粋ルール（Domain）

2) Infrastructure に入れる判断基準
# 入れるものの例

- 外部I/O：セーブ、ファイル、JSON/CSV読み込み、ネット
- 乱数実装、時計実装、ログ実装
- Addressables/Resourcesからロードする実装
- Unity依存でも「外部資源を取り出す/保存する」系

「外部の何かを読んだり書いたりしているか？」
「環境（Unity/OS/ストレージ）に依存しているか？」
「差し替えたい実装か？」（例：テストではMemorySave、実機ではFileSave）

# 入れないものの例

- HUD更新やアニメ再生（Presentation）
- ゲームルールそのもの（Domain）
- 進行制御（Application）

3) Presentation に入れる判断基準
# 入れるものの例

- UI（Text、Button、HUD）
- SE/BGM、アニメ、カメラ、フェード、シーン演出
- “見え方”に関するロジック（FogのGameObject生成・ON/OFFなど）
- Store購読してUIを更新する、みたいな“表示の反映”

「それは見た目/音/演出の都合か？」
「GameObjectやコンポーネントを触るか？」
「画面が変われば変更されるか？」

# 入れないものの例

- 勝敗やターンの規則（Domain）
- 進行制御（Application）
- 永続化/ファイル読み込み（Infrastructure）

4) Kernel（共通基盤）に入れる判断基準
# 入れるものの例

- どの層でも再利用する小物
- “特定のゲームルールに依存しない”汎用部品
- 例：Result<T>, Option<T>, AsyncCommandQueue<T>, DisposableBag みたいなやつ
- ただし “Unity依存の有無” でさらに分割することが多い
- Shared/Pure（Unityなし）
- Shared/Unity（UniTaskやUnity依存あり）

「これはテンプレとして他のゲームでも使える？」
「ルール変更ではなく、技術的都合で変わる？」
「層をまたいで使う可能性が高い？」


5) 迷ったときの最終決定フロー

- 1
 - ゲームルール（正しさ）が目的？ → Domain
 - 順番の制御（いつ何をするか）が目的？ → Application
 - ファイル/セーブ/ロード/外部資源に触る？ → Infrastructure
 - Unity型 or View都合がある？ → Presentation
 - どれにも当てはまらず、複数箇所で使う？ → Kernel

- 2
┌─ これは「ゲームの勝敗・ルール・状態」を決めるか？
│  ├─ YES → Domain
│  └─ NO ↓
│
├─ これは「いつ何をするか」の進行制御か？
│  ├─ YES → Application
│  └─ NO ↓
│
├─ これは「見た目・音・アニメ」など表示/演出か？
│  ├─ YES → Presentation
│  └─ NO ↓
│
├─ これは「ファイル・ネット・リソース」など外部I/Oか？
│  ├─ YES → Infrastructure
│  └─ NO ↓
│
└─ 複数の層で再利用する汎用部品か？
   ├─ YES → Kernel (Shared)
   └─ NO → どの層に属するか見直す

- 2
  - 質問1：Unity型を含むか？

GameObjectを生成/操作している       → Presentation または Infrastructure
AudioClip, Scene, Sprite を使う    → Presentation または Infrastructure
GetComponent, Find を使う           → Presentation または Infrastructure
Unityなしで動く（C#のみ）           → Domain, Application, または Kernel

  - 質問2：外部リソース（ファイル/ネット等）に触るか？
ファイル読み書き（セーブ/ロード）    → Infrastructure
JSON/CSV パース                     → Infrastructure
Addressables/Resources ロード       → Infrastructure
ネットワーク通信                    → Infrastructure
乱数生成、時計（実装）              → Infrastructure
  
どの外部リソースにも触らない        → Domain, Application, Presentation, Kernel

  - 質問3：「見た目・音・演出」に関するか？
UI更新（Text, Image, Button）      → Presentation
SE/BGM再生                          → Presentation
アニメーション、パーティクル        → Presentation
カメラ、フェード、シーン遷移演出    → Presentation
ゲームオブジェクトの表示/非表示      → Presentation
  
ルールや状態決定が目的              → Domain
表示のための単なる結果反映          → Presentation

  - 質問4：「複数の層やシステムを調停」しているか？
ユースケース実行（何ステップかの処理）    → Application
複数の Domain ロジックを組み合わせる      → Application
進行制御（ターン→判定→表示）             → Application
Domainの純粋なロジックだけ                → Domain
単一の責務                                → Domain, Presentation, Infrastructure

6) クイックリファレンス表
特徴              Domain Application Infrastructure Presentation Kernel
ゲーム勝敗/ルール  ✅    ❌          ❌             ❌          ❌
状態遷移          ✅    ❌          ❌             ❌          ❌
進行制御          ❌    ✅          ❌             ❌          ❌
UI更新            ❌    ❌          ❌             ✅          ❌
SE/BGM           ❌    ❌          ❌             ✅          ❌
ファイルI/O       ❌    ❌          ✅             ❌          ❌
ネット通信        ❌    ❌          ✅             ❌          ❌
Unity非依存      ✅    ❌          ❌             ❌           △
テスト可能        ✅    △           △             ❌           ✅
他ゲームで再利用  ❌    ❌          ❌             ❌           ✅

7) グレーゾーン解決ガイド

- ❓ Q1: 乱数（Random）はどこ？
// ❌ Domain に書く
public class Enemy
{
    public int GetDamage() => Random.Range(5, 10);  // ← ダメ
}

// ✅ Infrastructure インターフェイスを定義
// Kernel/Abstract/
public interface IRandom
{
    int Range(int min, int max);
}

// ✅ Domain は受け取る
public class Enemy
{
    private IRandom _rng;
    public Enemy(IRandom rng) { _rng = rng; }
    public int GetDamage() => _rng.Range(5, 10);  // OK
}

// ✅ Infrastructure で実装
public class UnityRandomImpl : IRandom
{
    public int Range(int min, int max) => Random.Range(min, max);
}

判定： インターフェイス → Kernel, 実装 → Infrastructure

- ❓ Q2: 時間計測（Time）はどこ？
// ❌ こういうのは Domain に入らない
public class BattleSystem
{
    public void Update()
    {
        _elapsedTime += Time.deltaTime;  // Unity型！
    }
}

// ✅ これ
// Kernel/Abstract/
public interface ITime
{
    float DeltaTime { get; }
    float Now { get; }
}

// Domain
public class BattleSystem
{
    private ITime _time;
    public void Update() => _elapsedTime += _time.DeltaTime;  // OK
}

// Infrastructure
public class UnityTimeImpl : ITime
{
    public float DeltaTime => Time.deltaTime;
    public float Now => Time.time;
}

判定： インターフェイス → Kernel, 実装 → Infrastructure

- ❓ Q3: ダメージ計算はどこ？
// ✅ Domain（ゲームのルール）
public class DamageCalculator
{
    public int Calculate(int attackPower, int defense)
    {
        return Mathf.Max(1, attackPower - defense / 2);
    }
}

// ✅ Domain（状態遷移も含む）
public class Character
{
    public int Health { get; private set; }
    
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
            Health = 0;  // ゲームのルール
    }
}

// ❌ これは Domain ではなく Application
public class BattleUseCase
{
    public void PlayerAttack(Character enemy)
    {
        // 複数のステップを組み合わせている
        var damage = _damageCalc.Calculate(_player.Attack, enemy.Defense);
        enemy.TakeDamage(damage);
        _soundService.PlaySFX("hit");  // 音も再生
        _uiService.ShowDamage(damage);  // UI更新
    }
}

判定： 純粋なルール → Domain, 複数ステップの実行 → Application

- ❓ Q4: キャラクターの移動はどこ？
// ✅ Domain（移動可否の判定）
public class GridMap
{
    public bool CanMove(Vector2Int from, Vector2Int to)
    {
        return IsInBounds(to) && !HasObstacle(to);
    }
}

// ✅ Application（移動の実行順序）
public class MoveUseCase
{
    public void Execute(Character character, Vector2Int targetPos)
    {
        if (!_map.CanMove(character.Position, targetPos))
            return;
        
        character.MoveTo(targetPos);
        _eventBus.Publish(new CharacterMovedEvent(character, targetPos));
    }
}

// ✅ Presentation（見た目の動き）
public class CharacterView : MonoBehaviour
{
    private void OnCharacterMoved(CharacterMovedEvent evt)
    {
        StartCoroutine(AnimateMovement(evt.To));
    }
    
    private IEnumerator AnimateMovement(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.Lerp(
                transform.position, target, Time.deltaTime * 5f);
            yield return null;
        }
    }
}

判定：
ルール → Domain
実行制御 → Application
見た目 → Presentation

- ❓ Q5: オブジェクトプール/リソース管理はどこ？
// ✅ Infrastructure（Unity依存 + リソース管理）
public class BulletPool : MonoBehaviour
{
    private Queue<Bullet> _available = new();
    
    public Bullet Get() { }
    public void Return(Bullet bullet) { }
}

// ✅ または Application（進行制御の一部なら）
public class ShootUseCase
{
    private readonly BulletPool _bulletPool;
    
    public void Execute(Vector3 shootPos, Vector3 direction)
    {
        var bullet = _bulletPool.Get();
        bullet.Launch(shootPos, direction);
    }
}

判定： リソース管理 → Infrastructure, 使用タイミング決定 → Application

- ❓ Q6: イベント/メッセージングはどこ？
// ✅ Kernel（汎用インフラ）
public interface IEvent { }
public class EventBus
{
    public void Publish<T>(T evt) where T : IEvent { }
    public void Subscribe<T>(Action<T> handler) where T : IEvent { }
}

// ✅ Kernel or Application（イベント定義）
public class PlayerHealthChangedEvent : IEvent
{
    public int NewHealth { get; set; }
}

// ✅ Domain/Application（イベント発行）
public class Character
{
    public void TakeDamage(int damage)
    {
        Health -= damage;
        _eventBus.Publish(new PlayerHealthChangedEvent { NewHealth = Health });
    }
}

// ✅ Presentation（イベント購読）
public class HealthBar : MonoBehaviour
{
    private void OnHealthChanged(PlayerHealthChangedEvent evt)
    {
        // UI更新
    }
}

判定： フレームワーク → Kernel, イベント定義 → Application, 購読側 → Presentation

8) よくある間違い
- ❌ 間違い1: Domain が Unity を触っている
public class BattleSystem
{
    public void Start()
    {
        var enemy = Instantiate(enemyPrefab);  // ← Domain 汚染！
    }
}
✅ 正解
// Domain
public class BattleSystem
{
    public void Start() { /* ルール実行 */ }
}

// Presentation
public class BattleView
{
    public void ShowBattle()
    {
        _battleSystem.Start();
        var enemy = Instantiate(enemyPrefab);  // ← ここで OK
    }
}

- ❌ 間違い2: Presentation が Application を決定している
public class Button : MonoBehaviour
{
    public void OnClick()
    {
        // UI層が進行制御を決めている
        var result = _battleSystem.Attack();
        _uiManager.ShowResult(result);
    }
}
✅ 正解
// Application
public class AttackUseCase
{
    public void Execute()
    {
        var result = _battleSystem.Attack();
        _eventBus.Publish(new AttackExecutedEvent(result));
    }
}

// Presentation
public class Button : MonoBehaviour
{
    public void OnClick() => _attackUseCase.Execute();
}

9) 実装例
ファイル                          → 層             → namespace
─────────────────────────────────────────────────────────────
Character.cs                     → Domain         → Game.Domain.Entities
BattleSystem.cs                  → Domain         → Game.Domain.GameLogic
DamageCalculator.cs              → Domain         → Game.Domain.GameLogic

AttackUseCase.cs                 → Application    → Game.Application.UseCases
BattleLoop.cs                    → Application    → Game.Application

CharacterView.cs                 → Presentation   → Game.Presentation.Characters
BattleUI.cs                      → Presentation   → Game.Presentation.UI

FileSave.cs                      → Infrastructure → Game.Infrastructure.Persistence
UnityRandomImpl.cs               → Infrastructure → Game.Infrastructure.Utils
UnityTimeImpl.cs                 → Infrastructure → Game.Infrastructure.Utils

Result<T>.cs                     → Kernel         → Game.Shared.Utils
ITime.cs                         → Kernel         → Game.Shared.Abstract
IRandom.cs                       → Kernel         → Game.Shared.Abstract
EventBus.cs                      → Kernel         → Game.Shared.Events
DisposableBag.cs                 → Kernel         → Game.Shared.Utils

10) 判定テンプレート
新しいクラスを作成するたびに：
## クラス名: ___________

### 質問
1. これはゲームのルール/勝敗を決めるか？ Y / N
2. これは「いつ何をするか」を制御するか？ Y / N
3. これはUI/音/見た目を担当するか？ Y / N
4. これはファイル/ネット/リソースに触るか？ Y / N
5. これは複数の層で再利用される？ Y / N

### 結論
このクラスは _________ 層に属する

### 検証
- [ ] 依存している層が正しいか確認
- [ ] 逆向きに依存していないか確認