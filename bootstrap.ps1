$root = Get-Location
$bootstrap = Join-Path $root "_bootstrap"
New-Item -name _bootstrap -type directory

$webclient = New-Object System.Net.WebClient
$url = "https://github.com/vigoo/bari/releases/download/0.8/bari-0.8.zip"
$file = Join-Path $bootstrap "bari.zip"
$webclient.DownloadFile($url,$file)

$shell_app=New-Object -com shell.application
$zip_file = $shell_app.namespace($file)
$destination = $shell_app.namespace($bootstrap)
$destination.Copyhere($zip_file.items())

