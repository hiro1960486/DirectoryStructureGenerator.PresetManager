PresetManager 製品版アイコン適用パッチ

【配置】
この IconPatch フォルダーを、次のファイルがあるプロジェクト直下へ置きます。
  DirectoryStructureGenerator.PresetManager.csproj

【実行】
1. IconPatch フォルダー内の apply-icon.ps1 を右クリック
2. 「PowerShellで実行」を選択

または、プロジェクト直下の PowerShell で次を実行します。
  powershell -ExecutionPolicy Bypass -File .\PresetManager_IconPatch_v1.1\apply-icon.ps1

【実行後】
プロジェクト直下に PresetManager_ProductIcon.ico がコピーされ、csproj に次が追加されます。
  <ApplicationIcon>PresetManager_ProductIcon.ico</ApplicationIcon>
  <Resource Include="PresetManager_ProductIcon.ico" />

【再ビルド】
  dotnet clean
  dotnet build
  .\publish-win-x64.cmd

※ Windows のアイコンキャッシュにより、古いアイコンが一時的に表示される場合があります。
