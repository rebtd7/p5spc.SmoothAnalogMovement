# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/SmoothAnalogMovement/*" -Force -Recurse
dotnet publish "./SmoothAnalogMovement.csproj" -c Release -o "$env:RELOADEDIIMODS/SmoothAnalogMovement" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location