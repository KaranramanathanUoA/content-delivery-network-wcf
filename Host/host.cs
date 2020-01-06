using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Threading;
using System.IO;

// How to: Create a Basic WCF Web HTTP Service
// https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-create-a-basic-wcf-web-http-service

// How to: Expose a Contract to SOAP and Web Clients
// https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-expose-a-contract-to-soap-and-web-clients

[ServiceContract()]
public interface IClientAndServerService
{
    [OperationContract()]
    [WebGet()]
    string[] DisplayFiles();

    [OperationContract()]
    [WebGet()]
    byte[] DownloadFiles(string fileName);
}

[ServiceBehavior(
    InstanceContextMode=InstanceContextMode.Single, 
    ConcurrencyMode=ConcurrencyMode.Multiple)] 
public class HostService : IClientAndServerService
{
    public static Func<int> Tid = () => Thread.CurrentThread.ManagedThreadId;
        
    public static Func<double> Millis = () => DateTime.Now.TimeOfDay.TotalMilliseconds;

    public string[] DisplayFiles() {
        string path = Directory.GetCurrentDirectory();
        string ServerFiles = Path.Combine(path, "ServerFiles");
        string[] filesInDirectory = Directory.GetFiles(ServerFiles);
        return filesInDirectory;
    }

    public byte[] DownloadFiles(string fileName) {
        string path = Directory.GetCurrentDirectory();
        string combined = Path.Combine(path, "ServerFiles");
        string finalpath = Path.Combine(combined, fileName);
        byte[] buffer = System.IO.File.ReadAllBytes(finalpath);
        return buffer;

    }
}

public class Host {
    public static void Main() {
        Uri baseAddress = new Uri("http://localhost:8082/hello");

        WebServiceHost host = null;

        try {
            host = new WebServiceHost(typeof(HostService), baseAddress);
            ServiceEndpoint ep = host.AddServiceEndpoint(typeof(IClientAndServerService), new WebHttpBinding(), "");

            host.Open();

            Console.WriteLine($"The service is ready at {baseAddress}");
            Console.WriteLine("Press <Enter> to stop the service.");
            Console.ReadLine();

            // Close the ServiceHost.
            host.Close();
        
        } catch (Exception ex) {
            Console.WriteLine($"*** Exception {ex.Message}");
            host = null;
        
        } finally {
            if (host != null) ((IDisposable)host).Dispose();
        }
    }
}
