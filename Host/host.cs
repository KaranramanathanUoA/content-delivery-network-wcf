using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Threading;
using System.IO;

// How to: Expose a Contract to SOAP and Web Clients
// https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-expose-a-contract-to-soap-and-web-clients

/* Service contract between a client and host that defines the supported operations by the WCF application. In this case, we can display the files 
   hosted on the server as well as download it. Refer to https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-create-a-basic-wcf-web-http-service 
   for details on WCF applications. */
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
    public string[] DisplayFiles() {
        string path = Directory.GetCurrentDirectory();
        string ServerFiles = Path.Combine(path, "ServerFiles");
        // Gets the files present in the 'ServerFiles' folder present in the host folder
        string[] filesInDirectory = Directory.GetFiles(ServerFiles);
        return filesInDirectory;
    }

    public byte[] DownloadFiles(string fileName) {
        string path = Directory.GetCurrentDirectory();
        string combined = Path.Combine(path, "ServerFiles");
        string finalpath = Path.Combine(combined, fileName);
        // Reads the file present in the server and returns contents as a byte stream
        byte[] buffer = System.IO.File.ReadAllBytes(finalpath);
        return buffer;

    }
}

public class Host {
    public static void Main() {
        // Create an instance of the Uri class to hold the base address of the service
        Uri baseAddress = new Uri("http://localhost:8082/hello");
        WebServiceHost host = null;

        try {
            host = new WebServiceHost(typeof(HostService), baseAddress);
            // Adds a IClientAndServerService end point with webHttpBinding
            ServiceEndpoint ep = host.AddServiceEndpoint(typeof(IClientAndServerService), new WebHttpBinding(), "");
            // Host in now actively listening for any requests made to this endpoint
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