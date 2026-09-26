# DirectoryStructureGenerator.PresetManager

プリセットCSVを管理する Windows デスクトップアプリです。Directory Structure Generator の整理コピー設定を編集し、CSV の保存先を切り替えられます。

**Version:** 1.1  
**Platform:** Windows 10/11 (x64)  
**Framework:** .NET 8 / WPF

## 主な機能

- プリセットの追加・編集・比較
- CSV の保存先を任意の場所へ変更
- 保存先の切り替え時に既存CSVを採用、現在のデータをコピー、または空CSVを作成
- 上書き前の日時付きバックアップ
- 一時ファイルを使った設定・CSVの安全な置き換え

## 開発環境での起動

.NET 8 SDK と Windows が必要です。リポジトリのルートで実行します。

```powershell
dotnet restore
dotnet build -c Release
dotnet run
```

`start.cmd` からも起動できます。

## Windows x64 配布用ビルド

```powershell
.\publish-win-x64.cmd
```

自己完結型の配布ファイルは `publish\win-x64` に生成されます。この生成先と `bin` / `obj` はGit管理対象外です。GitHubで配布するときは、配布物をソースとは分けて Release に添付してください。

## 設定ファイル

- `Config/presets.csv`: 初期プリセット
- `Config/settings.json`: プリセットCSVの保存先

アプリ配下の保存先は相対パス、外部ドライブ・NASなどの保存先は絶対パスで記録されます。

## ドキュメント

操作マニュアルなどの資料は後日追加予定です。
