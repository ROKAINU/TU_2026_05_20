# Memo

### セットアップ
・check-meta-files.ymlの全ての"Template/Assets"を"現在のプロジェクト名/Assets"に変更する

### Git LFS を必ず使用する
理由：FontのAssetが100MBを超えるため

### コマンド
git lfs install
git lfs track

Unity Game Template
コーディング規約について
このテンプレートは自動的に以下を適用します。
自動適用される設定

エディタ設定（.editorconfig）
コードスニペット（state, usecase, iservice）

### スニペット使用方法
入力生成されるものstate + TabState パターンのクラスusecase + TabUseCase パターンのクラスiservice + TabService インターフェース

###  コード自動修正（オプション）
bash# フォーマットをまとめて修正したい場合
dotnet format

将来、チーム開発に移行するとき
以下を必要に応じて追加してください。

Roslyn Analyzer（コード品質チェック）
pre-commit hook（commit前の自動チェック）
GitHub Actions（CI/CDでの自動検証）

個人開発の段階ではこれらは不要です。