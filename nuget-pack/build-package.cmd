@echo off

set MSBuild=C:\Program Files (x86)\MSBuild\14.0\Bin

set path=%path%;%MSBuild%

@echo on

msbuild /property:Configuration="Release"

copy RpcLite.1.0.1.9.nupkg E:\Users\Chris\Desktop\NuGet\NuGet.Server\src\NuGet.Server\Packages\

pause