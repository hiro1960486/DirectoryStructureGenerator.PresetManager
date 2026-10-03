# Directory Structure Generator - Preset Manager

## 統合APP「FileWorkbench」への移行予告（2026-10-03）

現在、次の3つのAPPの機能を統合する「FileWorkbench」を開発中です。

- DirectoryStructureGenerator：フォルダー構成の出力、ファイル情報の確認・整理
- DirectoryStructureGenerator.PresetManager：プリセット設定の管理
- RenameWizard：ファイル名の変更、コピー・移動

今後は統合APP「FileWorkbench」へ移行する予定です。あわせて、上記3つのAPPはそれぞれ単独APPとしても改善・開発を継続します。正式公開・移行開始の案内は、準備が整い次第お知らせします。

初心者向けの入手・ビルドガイドと、開発中の画面資料も整備しています。現在は開発段階の予告であり、既存設定・データの移行方法や互換性は、今後の案内でお知らせします。


![Preset Manager product icon](docs/images/product-icon.png)

プリセットCSVを管理する Windows デスクトップアプリです。Directory Structure Generator の整理コピー設定を登録・比較し、プリセットCSVの保存先を切り替えられます。

**アプリ版:** 1.1.2

**対応OS:** Windows 10 / 11 (x64)

**開発環境:** .NET 8 / WPF

## ダウンロード

[Download Latest Release](https://github.com/hiro1960486/DirectoryStructureGenerator.PresetManager/releases/latest)

Releaseから `DirectoryStructureGenerator.PresetManager_v1.1.2_win-x64.zip` をダウンロードし、フォルダーへ展開して `DirectoryStructureGenerator.PresetManager.exe` を起動してください。配布ZIPには実行に必要なファイル一式が含まれています。.NETの別途インストールは不要です。

## 主な機能

- 整理コピー用プリセットの追加、編集、比較
- プリセットをCSVで管理
- CSVの保存先をアプリ内、別ドライブ、OneDrive、NASなどへ変更
- 保存先変更時に、現在のデータをコピー、既存CSVを使用、空CSVを作成から選択
- 上書き前の既存CSVバックアップと、一時ファイル経由の設定保存
- 表示順は数字を入力して Enter で確定します。入力中の数字では設定が変わりません。
- 画面上の用語を Directory Structure Generator と合わせ、「対象フォルダー」「出力先」に統一

## プリセットの編集

1. 一覧からプリセットを選び、「編集」を押します。
2. 「基本情報」で名称、メモ、順番を確認します。順番の数字を変更したら Enter を押して確定します。
3. 「実行情報」では「対象フォルダー」と「出力先」を指定できます。対象フォルダーが空欄なら実行時に選び、出力先が空欄なら Directory Structure Generator の既定値を使います。
4. 「重複チェックして保存」を押して確定します。

## 保存先を変更するとき

1. 現在のプリセットデータを保存します。
2. 新しいCSVの保存先を選びます。
3. 保存先の状態に合わせて移行方法を選びます。
   - **現在のデータをコピー:** 現在のプリセットで保存先CSVを作成または置き換えます。
   - **既存CSVを使用:** 選択先にあるCSVを読み込みます。ファイルがない場合は変更できません。
   - **空CSVを作成:** ヘッダーのみのCSVを作成します。
4. データ側の処理が成功した後に、`Config/settings.json` の保存先設定を更新します。

元のCSVは自動削除しません。置き換え対象に同名のCSVがある場合は、日時を付けた `.backup_YYYYMMDD_HHMMSS` ファイルを作ります。設定ファイルの置き換え時も、既存設定のバックアップを作ります。必要に応じて、作業前にCSVを別の場所へコピーしてください。

## 設定ファイル

- `Config/presets.csv`: 初期プリセット
- `Config/settings.json`: 現在使うCSVのパス

既定値は次のとおりです。

```json
{
  "presetFilePath": "Config\\presets.csv"
}
```

アプリのフォルダー内は相対パス、外部ドライブやNASなどは絶対パスで保存します。外部の保存先へアクセスできない場合は、ドライブやネットワーク接続、フォルダーへの書き込み権限を確認してください。

## 画面資料・マニュアル

メイン画面や保存先変更画面のスクリーンショット、操作マニュアルは後日追加予定です。画面資料を追加するときは、個人名、ユーザーフォルダー名、メールアドレス、NASパスなどが写っていないことを確認します。

## 開発・ビルド

Windows と .NET 8 SDK を用意し、リポジトリのルートで PowerShell を開いて実行します。

```powershell
dotnet restore
dotnet clean
dotnet build -c Release
dotnet run
```

自己完結型の Windows x64 配布物を作る場合は、次を実行します。

```powershell
.\publish-win-x64.cmd
```

生成物は `publish\win-x64` に作られます。配布するときは、このフォルダー内のファイルをまとめてZIPにします。`bin`、`obj`、`publish`はGit管理対象外です。

## リポジトリ構成

```text
Config/       初期CSVと既定設定
Models/       データモデル
Services/     CSV、設定、保存先移行の処理
ViewModels/   画面状態と操作処理
Views/        WPF画面
docs/         画面資料、マニュアル、設計資料（準備中）
```

## バージョン

- `PATCH`: 文書・公開ファイルの整理、不具合修正
- `MINOR`: 後方互換性を保つ機能追加
- `MAJOR`: 既存設定やデータとの互換性を壊す変更

既存のCSV列構成と `settings.json` の保存先切替仕様を維持します。

## ライセンス

[MIT License](LICENSE)
