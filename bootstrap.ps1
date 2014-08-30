Write-Output "Installing Chocolatey"

iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))

Write-Output "Installing wget"

cinst Wget

Write-Output "Installing 7zip"

cinst 7zip.commandline

Write-Output "Creating _bootstrap directory"

$root = Get-Location
$bootstrap = Join-Path $root "_bootstrap"
New-Item -name _bootstrap -type directory -force

Set-Alias zip "7za.exe"

$barizip = Join-Path $bootstrap "bari.zip"

Write-Output "Downloading bari"

wget --no-check-certificate "https://github.com/vigoo/bari/releases/download/0.8/bari-0.8.zip" -O $barizip

Write-Output "Unpacking bari"

zip x $barizip -o_bootstrap
