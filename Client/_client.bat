if exist client.exe del client.exe
csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll client.cs
pause

client.exe
pause

client.exe
pause
