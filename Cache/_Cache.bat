if exist Cache.exe del Cache.exe
csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll Cache.cs
pause

Cache.exe
pause
