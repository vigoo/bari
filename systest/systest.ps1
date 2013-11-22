
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

Function InternalCheckExe($name, $path, $exitcode, $output)
{
    $code = (Start-Process -FilePath $path -RedirectStandardOutput "..\tmp\$name.out" -Wait -NoNewWindow -PassThru).ExitCode
    if ($code -eq $exitcode) 
    {
        if ((get-content "..\tmp\$name.out") -eq $output) 
        {
            return $true
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

    return $false
}

Function CheckExe($name, $path, $exitcode, $output)
{
    $res = (InternalCheckExe $name $path $exitcode $output)
    if ($res -eq $true)
    {
        Write-Host "OK"        
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

Function ContentTest
{
    Write-Host "..content-test.." -NoNewline
    Push-Location -Path "content-test"
    Clean "content-test"
    Build "content-test"
    $res = (InternalCheckExe "content-test" "target\HelloWorld\HelloWorld.exe" 11 "Test executable running")
    Pop-Location    

    if ($res -eq $true) 
    {
        if ((get-content "content-test\target\HelloWorld\content-beside-cs.txt") -eq "content-beside-cs")             
        {
            if ((get-content "content-test\target\HelloWorld\additional-content.txt") -eq "additional-content")             
            {           
                Write-Host "OK"
            }
            else 
            {
                Write-Host "FAIL (wrong content in additional-content.txt)"    
            }
        }
        else 
        {
            Write-Host "FAIL (wrong content in content-beside-cs.txt)"    
        }
    }
}

Write-Host "Executing system tests for bari..."
Initialize

SimpleExeBuild "single-cs-exe" "target\HelloWorld\HelloWorld.exe" 11 "Test executable running"
SimpleExeBuild "single-fs-exe" "target\Module\Exe1.exe" 12 "Test F# executable running"
SimpleExeBuild "single-cpp-exe" "target\Module1\hello.exe" 13 "Test C++ executable running"
SimpleExeBuild "module-ref-test" "target\HelloWorld\HelloWorld.exe" 10 "TEST"
SimpleExeBuild "suite-ref-test" "target\HelloWorld\HelloWorld.exe" 10 "TEST"
SimpleExeBuild "fsrepo-test" "target\HelloWorld\HelloWorld.exe" 9 "Dependency acquired"
SimpleExeBuild "alias-test" "target\HelloWorld\HelloWorld.exe" 9 "Dependency acquired"
ContentTest
SimpleExeBuild "runtime-ref-test" "target\HelloWorld\HelloWorld.exe" 0 $null
SimpleExeBuild "regfree-com-server" "target\client\comclient.exe" 0 "Hello world"
SimpleExeBuild "script-test" "target\HelloWorld\HelloWorld.exe" 11 "Hello world!!!"
