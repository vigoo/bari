
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

Function CleanWithGoal($name, $goal)
{
    Start-Process -FilePath "..\..\target\full\bari.exe" -ArgumentList "-v --target $goal clean" -RedirectStandardOutput "..\logs\$name.clean.log" -Wait -NoNewWindow    
}

Function Build($name)
{
    Start-Process -FilePath "..\..\target\full\bari.exe" -ArgumentList "-v build" -RedirectStandardOutput "..\logs\$name.build.log" -Wait -NoNewWindow    
}

Function BuildWithGoal($name, $goal)
{
    Start-Process -FilePath "..\..\target\full\bari.exe" -ArgumentList "-v --target $goal build" -RedirectStandardOutput "..\logs\$name.build.log" -Wait -NoNewWindow    
}

Function BuildProduct($name, $product)
{
    Start-Process -FilePath "..\..\target\full\bari.exe" -ArgumentList "-v build $product" -RedirectStandardOutput "..\logs\$name.build.log" -Wait -NoNewWindow        
}

Function InternalCheckExe($name, $path, $exitcode, $output)
{
    $code = (Start-Process -FilePath $path -RedirectStandardOutput "..\tmp\$name.out" -Wait -NoNewWindow -PassThru).ExitCode
    if ($code -eq $exitcode) 
    {
        if (((get-content "..\tmp\$name.out") -join "`n") -eq $output) 
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

Function ExeProductBuild($name, $product, $path, $exitcode, $output)
{
    Write-Host "..$name.." -NoNewline
    Push-Location -Path $name
    Clean $name
    BuildProduct $name $product
    CheckExe $name $path $exitcode $output
    Pop-Location    
}

Function ExeBuildWithGoal($name, $goal, $path, $exitcode, $output)
{
    Push-Location -Path $name
    CleanWithGoal $name $goal
    BuildWithGoal $name $goal
    $res = (InternalCheckExe $name $path $exitcode $output)
    Pop-Location    

    return $res
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

Function X86X64Test
{
    Write-Host "..x86-x64-test.." -NoNewline
    $res1 = (ExeBuildWithGoal "x86-x64-test" "debug-x86" "target\HelloWorld\HelloWorld.exe" 32 "32 bit")
    $res2 = (ExeBuildWithGoal "x86-x64-test" "debug-x64" "target\HelloWorld\HelloWorld.exe" 64 "64 bit")

    if ($res1 -eq $true) 
    {
        if ($res2 -eq $true)
        {
            Write-Host "OK"
        }
    }
}

Function CppReleaseTest
{
    Write-Host "..cpp-release-test.." -NoNewline
    $res = (ExeBuildWithGoal "cpp-release-test" "custom-release" "target\Module1\hello.exe" 13 "Test C++ executable running")

    if ($res -eq $true) 
    {
        Write-Host "OK"
    }
}

Function MultiSolutionTest
{
    Write-Host "..multi-solution-test.." -NoNewline

    Push-Location -Path "suite-ref-test"
    Clean "multi-solution-test1.1"
    BuildProduct "multi-solution-test1.1" "all"
    $res1 = (InternalCheckExe "multi-solution-test1.1" "target\HelloWorld\HelloWorld.exe" 10 "TEST")

    Get-ChildItem "target" | Remove-Item -Recurse -Force 

    BuildProduct "multi-solution-test1.2" "HelloWorld"
    $res2 = (InternalCheckExe "multi-solution-test1.2" "target\HelloWorld\HelloWorld.exe" 10 "TEST")

    Get-ChildItem "target" | Remove-Item -Recurse -Force 

    BuildProduct "multi-solution-test1.3" "all"
    $res3 = (InternalCheckExe "multi-solution-test1.3" "target\HelloWorld\HelloWorld.exe" 10 "TEST")

    Pop-Location

    if ($res1 -and $res2 -and $res3)
    {
        Write-Host "OK"
    }
}


Function MultiSolutionTest2
{
    Write-Host "..multi-solution-test2.." -NoNewline

    Push-Location -Path "multi-solution-test2"
    Clean "multi-solution-test2.1"
    BuildProduct "multi-solution-test2.1" "all"
    $res1 = (InternalCheckExe "multi-solution-test2.1" "target\Mod1\Proj11.exe" 10 "1234")

    Get-ChildItem "target" | Remove-Item -Recurse -Force 

    BuildProduct "multi-solution-test2.2" "Mod1"
    $res2 = (InternalCheckExe "multi-solution-test2.2" "target\Mod1\Proj11.exe" 10 "1234")

    Get-ChildItem "target" | Remove-Item -Recurse -Force 

    BuildProduct "multi-solution-test2.3" "all"
    $res3 = (InternalCheckExe "multi-solution-test2.3" "target\Mod1\Proj11.exe" 10 "1234")

    Pop-Location

    if ($res1 -and $res2 -and $res3)
    {
        Write-Host "OK"
    }
}

Write-Host "Executing system tests for bari..."
Initialize

SimpleExeBuild "single-cs-exe" "target\HelloWorld\HelloWorld.exe" 11 "Test executable running"
SimpleExeBuild "single-fs-exe" "target\Module\Exe1.exe" 12 "Test F# executable running"
SimpleExeBuild "single-cpp-exe" "target\Module1\hello.exe" 13 "Test C++ executable running"
SimpleExeBuild "module-ref-test" "target\HelloWorld\HelloWorld.exe" 10 "TEST"
SimpleExeBuild "module-ref-test-withrt" "target\HelloWorld\HelloWorld.exe" 10 "TEST"
SimpleExeBuild "suite-ref-test" "target\HelloWorld\HelloWorld.exe" 10 "TEST"
SimpleExeBuild "fsrepo-test" "target\HelloWorld\HelloWorld.exe" 9 "Dependency acquired"
SimpleExeBuild "alias-test" "target\HelloWorld\HelloWorld.exe" 9 "Dependency acquired"
ContentTest
SimpleExeBuild "runtime-ref-test" "target\HelloWorld\HelloWorld.exe" 0 ""
SimpleExeBuild "regfree-com-server" "target\client\comclient.exe" 0 "Hello world"
SimpleExeBuild "script-test" "target\HelloWorld\HelloWorld.exe" 11 "Hello base!!!`n`nHello world!!!`n"
SimpleExeBuild "mixed-cpp-cli" "target\Module1\hello.exe" 11 "Hello world"
SimpleExeBuild "static-lib-test" "target\test\hello.exe" 10 "Hello world!"
SimpleExeBuild "cpp-rc-support" "target\Module1\hello.exe" 13 "Test C++ executable running"
X86X64Test
SimpleExeBuild "embedded-resources-test" "target\HelloWorld\HelloWorld.exe" 11 "Hello world!`nWPF hello world WPF!"
ExeProductBuild "postprocessor-script-test" "main" "target\main\HelloWorld.exe" 11 "Hello world`n!!!`n"
CppReleaseTest
SimpleExeBuild "cpp-version" "target\Module1\hello.exe" 11 "1.2.3.4`n1.2.3.4"
SimpleExeBuild "custom-plugin-test" "target\HelloWorld\HelloWorld.exe" 11 "Hello base!!!`n`nHello world!!!`n"
MultiSolutionTest
MultiSolutionTest2