$ErrorActionPreference = "Stop"

$projectDirectory = "$PSScriptRoot\Community.PowerToys.Run.Plugin.Timer"
[xml]$xml = Get-Content -Path "$projectDirectory\Community.PowerToys.Run.Plugin.Timer.csproj"
$version = $xml.Project.PropertyGroup.Version
$version = "$version".Trim()

foreach ($platform in "ARM64", "x64")
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

    Compress-Archive -Path "$projectDirectory\bin\$platform\Timer" -DestinationPath "$PSScriptRoot\Timer-$version-$platform.zip"
}
