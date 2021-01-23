$OrigiPath = "$((Get-Location).Path)"

dotnet --info

try {
    Set-Location $PSScriptRoot

    $CurrentPath = "$((Get-Location).Path)"
    Write-Host "CurrentPath $CurrentPath"

    $AfactsPath = "$CurrentPath\artifacts"
    Write-Host "AfactsPath $AfactsPath"

    if ( -Not (Test-Path -Path $AfactsPath ) ) {
        Write-Host "Artifact directory not exists, exit"
        return
    }

    $ArtifactFiles = Get-ChildItem -Path $AfactsPath

    for ($i = 0; $i -lt $ArtifactFiles.Count; $i++) {
        $artifact = $ArtifactFiles[$i]
        # BuildPackage -dir $codePath
        # Set-Location $CurrentPath
        Write-Host $artifact.FullName
        try {
            nuget push $artifact.FullName -Source https://api.nuget.org/v3/index.json
        }
        catch {
        
        }
    }
}
catch {
    
}
finally {
    Write-Host "Finish Publish"
    Set-Location $OrigiPath   
}