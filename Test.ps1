$ErrorActionPreference = "Stop"

$projectDirectory = "$PSScriptRoot\Community.PowerToys.Run.Plugin.Timer"
[xml]$xml = Get-Content -Path "$projectDirectory\Community.PowerToys.Run.Plugin.Timer.csproj"
$version = $xml.Project.PropertyGroup.Version
$version = "$version".Trim()

foreach ($platform in "x64")
{
    if (Test-Path -Path "$projectDirectory\bin")
    {
        Remove-Item -Path "$projectDirectory\bin\*" -Recurse
    }

    if (Test-Path -Path "$projectDirectory\obj")
    {
        Remove-Item -Path "$projectDirectory\obj\*" -Recurse
    }

    dotnet build $projectDirectory.sln -c Release /p:Platform=$platform

    Remove-Item -Path "$projectDirectory\bin\*" -Recurse -Include *.xml, *.pdb, PowerToys.*, Wox.*
    Rename-Item -Path "$projectDirectory\bin\$platform\Release" -NewName "Timer"

    # now we need to find the path of the process
    # then store the path in a variable
    # then stop the process
    # then restart the process

    # get the process
    $process = Get-Process -Name "PowerToys.PowerLauncher"

    # get the path of the process
    $processPath = $process.Path

    # stop the process
    Stop-Process -Name "PowerToys.PowerLauncher"

    # wait for the process to stop
    while (Get-Process -Name "PowerToys.PowerLauncher" -ErrorAction SilentlyContinue) {
        Start-Sleep -Seconds 1
        Write-Progress -Activity "Waiting for PowerToys.PowerLauncher to stop" -Status "Progress"
    }

    # overwrite the content in the pluging folder "%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\Timer"
    Copy-Item -Path "$projectDirectory\bin\$platform\Timer" -Destination "$env:LOCALAPPDATA\Microsoft\PowerToys\PowerToys Run\Plugins" -Recurse -Force

    # start the process
    Start-Process -FilePath $processPath

    # wait for the process to start with a progress count

    while ($process -eq $null) {
        Start-Sleep -Seconds 1
        $process = Get-Process -Name "PowerToys.PowerLauncher" -ErrorAction SilentlyContinue
        Write-Progress -Activity "Waiting for PowerToys.PowerLauncher to start" -Status "Progress"
    }

}
