$ErrorActionPreference = 'Stop'
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$htmlSource = Join-Path $scriptDir '..\scp_3999_scipnet_edit_v4_0.html'
$csc = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$outExe = Join-Path $scriptDir 'SCP-3999.exe'
$iconPath = Join-Path $scriptDir 'app.ico'

if (-not (Test-Path $htmlSource)) { throw "Missing source HTML: $htmlSource" }
if (-not (Test-Path $csc)) { throw "Missing C# compiler: $csc" }

$resolvedHtml = (Resolve-Path $htmlSource).Path
$iconArg = if (Test-Path $iconPath) { "/win32icon:`"$iconPath`"" } else { "" }
& $csc /nologo /target:winexe /optimize+ $iconArg /out:"$outExe" /resource:"$resolvedHtml,scp_3999.html" "$scriptDir\Program.cs"
if ($LASTEXITCODE -ne 0) { throw "Compilation failed with exit code $LASTEXITCODE" }

$info = Get-Item $outExe
Write-Output ("Built: {0} ({1:N1} KB)" -f $info.FullName, ($info.Length / 1KB))
