$assetDirPath = Join-Path $PSScriptRoot "assets"
$asmPath = Join-Path $assetDirPath "test.dll"
$srcPath = Join-Path $assetDirPath "testsource.cs"

if (Test-Path $asmPath)
{
    Remove-Item $asmPath
}

Add-Type -Path $srcPath -OutputAssembly $asmPath -CompilerOptions "/unsafe" -IgnoreWarnings

try
{
    Push-Location $PSScriptRoot
    dotnet test
}
finally
{
    Pop-Location
}