
Function Initialize
{
    Write-Host "..initializing.." -NoNewline
    Get-ChildItem "logs" | Remove-Item -Recurse -Force 
    Get-ChildItem "tmp" | Remove-Item -Recurse -Force

    if (!(Test-Path -Path "logs")) 
    {
        New-Item -Type 'directory' -Path "logs"
    }
    if (!(Test-Path -Path "tmp")) 
    {
        New-Item -Type 'directory' -Path "tmp"
    }
    Write-Host "done!"
}

Function Clean($name)
{
    Start-Process -FilePath "..\..\target\full\bari.exe" -ArgumentList "-v clean" -RedirectStandardOutput "..\logs\$name.clean.log" -Wait -NoNewWindow    
}

Function Build($name)
{
    Start-Process -FilePath "..\..\target\full\bari.exe" -ArgumentList "-v build" -RedirectStandardOutput "..\logs\$name.build.log" -Wait -NoNewWindow    
}

Function CheckExe($name, $path, $exitcode, $output)
{
    $code = (Start-Process -FilePath $path -RedirectStandardOutput "..\tmp\$name.out" -Wait -NoNewWindow -PassThru).ExitCode
    if ($code -eq $exitcode) 
    {
        if ((get-content "..\tmp\$name.out") -eq $output) 
        {
            Write-Host "OK"
        }
        else 
        {
            Write-Host "FAIL (wrong standard output)"    
        }
    }
    else 
    {
        Write-Host "FAIL (exit code was $code)"
    }
}

Function SimpleExeBuild($name, $path, $exitcode, $output)
{
    Write-Host "..$name.." -NoNewline
    Push-Location -Path $name
    Clean $name
    Build $name
    CheckExe $name $path $exitcode $output
    Pop-Location    
}

Write-Host "Executing system tests for bari..."
Initialize

SimpleExeBuild "single-cs-exe" "target\HelloWorld\HelloWorld.exe" 11 "Test executable running"
SimpleExeBuild "single-fs-exe" "target\Module\Exe1.exe" 12 "Test F# executable running"
SimpleExeBuild "single-cpp-exe" "target\Module1\hello.exe" 13 "Test C++ executable running"
SimpleExeBuild "fsrepo-test" "target\HelloWorld\HelloWorld.exe" 9 "Dependency acquired"
