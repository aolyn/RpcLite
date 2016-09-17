call build-env-set.cmd

md bin\net40

copy RpcLite.NetFx\bin\%cfg%\net40\* bin\net40\

pause