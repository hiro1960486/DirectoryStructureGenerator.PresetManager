$ErrorActionPreference = 'Stop'

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = Split-Path -Parent $scriptDir
$iconName = 'PresetManager_ProductIcon.ico'
$sourceIcon = Join-Path $scriptDir $iconName
$projectIcon = Join-Path $projectDir $iconName
$projectFile = Get-ChildItem -Path $projectDir -Filter '*.csproj' -File | Select-Object -First 1

if (-not $projectFile) {
    throw "プロジェクトファイル（.csproj）が見つかりません。IconPatchフォルダーをプロジェクト直下へ置いてください。"
}

Copy-Item -LiteralPath $sourceIcon -Destination $projectIcon -Force

[xml]$xml = Get-Content -LiteralPath $projectFile.FullName -Raw
$ns = $xml.Project.NamespaceURI

function New-ProjectElement([string]$name) {
    if ([string]::IsNullOrEmpty($ns)) {
        return $xml.CreateElement($name)
    }
    return $xml.CreateElement($name, $ns)
}

$propertyGroup = @($xml.Project.PropertyGroup) | Where-Object { $_.ApplicationIcon } | Select-Object -First 1
if (-not $propertyGroup) {
    $propertyGroup = New-ProjectElement 'PropertyGroup'
    [void]$xml.Project.AppendChild($propertyGroup)
}

if ($propertyGroup.ApplicationIcon) {
    $propertyGroup.ApplicationIcon = $iconName
} else {
    $applicationIcon = New-ProjectElement 'ApplicationIcon'
    $applicationIcon.InnerText = $iconName
    [void]$propertyGroup.AppendChild($applicationIcon)
}

$existingResource = @($xml.Project.ItemGroup.Resource) | Where-Object { $_.Include -eq $iconName } | Select-Object -First 1
if (-not $existingResource) {
    $itemGroup = New-ProjectElement 'ItemGroup'
    $resource = New-ProjectElement 'Resource'
    [void]$resource.SetAttribute('Include', $iconName)
    [void]$itemGroup.AppendChild($resource)
    [void]$xml.Project.AppendChild($itemGroup)
}

$settings = New-Object System.Xml.XmlWriterSettings
$settings.Indent = $true
$settings.Encoding = New-Object System.Text.UTF8Encoding($false)
$writer = [System.Xml.XmlWriter]::Create($projectFile.FullName, $settings)
try {
    $xml.Save($writer)
} finally {
    $writer.Dispose()
}

Write-Host ''
Write-Host 'アイコンをプロジェクトへ設定しました。' -ForegroundColor Green
Write-Host "Project: $($projectFile.FullName)"
Write-Host "Icon:    $projectIcon"
Write-Host ''
Write-Host '次のコマンドで再ビルドしてください。'
Write-Host 'dotnet clean'
Write-Host 'dotnet build'
Write-Host '.\publish-win-x64.cmd'
