set tbs_sdkVersion=4.3.0
set tbs_sdk_withfilereaderVersion=3.2.0

msbuild ../tbs_sdk/tbs_sdk.csproj /t:Rebuild /p:Configuration=Release
msbuild ../tbs_sdk_withfilereader/tbs_sdk_withfilereader.csproj /t:Rebuild /p:Configuration=Release

nuget pack tbs_android_sdk.nuspec -Version %tbs_sdkVersion%
nuget pack tbs_sdk_withfilereader.nuspec -Version %tbs_sdk_withfilereaderVersion%