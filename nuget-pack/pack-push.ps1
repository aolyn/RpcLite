$OrigiPath = "$((Get-Location).Path)"

Write-Host $PSScriptRoot
Set-Location $PSScriptRoot

try {
    .\pack.ps1
    .\nuget-push.ps1
}
catch {
    
}

Set-Location $OrigiPath