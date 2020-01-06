if exist host.exe del host.exe
csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll host.cs
pause

host.exe
pause
