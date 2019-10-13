dotnet --info

Set-Location "nuget-pack"

$CurrentPath = "$((Get-Location).Path)"

$AfactsPath = "$CurrentPath\artifacts"

if ( -Not (Test-Path -Path $AfactsPath ) ) {
    New-Item -ItemType directory -Path $AfactsPath
}
else {
    Remove-Item "$AfactsPath\*.nupkg"
}

function BuildPackage($dir) {     
    Set-Location $dir
    dotnet build -c Debug
    Remove-Item bin/Debug/*.nupkg
    dotnet pack -c Debug
    Copy-Item "bin/Debug/*.nupkg" $AfactsPath
}

[string[]]$CodePaths = "../src/RpcLite", `
    "../src/RpcLite.AspNetCore", `
    "../src/RpcLite.Formatters.Protobuf", `
    "../src/RpcLite.NetFx", `
    "../src/RpcLite.Server.Kestrel"

for ($i = 0; $i -lt $CodePaths.Count; $i++) {
    $codePath = $CodePaths[$i]
    BuildPackage -dir $codePath
    Set-Location $CurrentPath
}

#Write-Host "Start push packages"
#Set-Location $AfactsPath
#$packages = Get-ChildItem -Filter "*.nupkg"
#ForEach ($item in $packages) {
#    $pakcagePath = $item.FullName
#    Write-Host $pakcagePath
#
#    nuget push "$pakcagePath" -Source https://api.nuget.org/v3/index.json -ApiKey ${env:NUGET_API_KEY_ORIGIN}
#}
Write-Host "Finish"