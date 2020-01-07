using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

[ServiceContract()]
public interface ICacheService
{
    [OperationContract()]
    [WebGet()]
    string[] DisplayFiles();

    [OperationContract()]
    [WebGet()]
    byte[] DownloadFiles(string fileName);
}

[ServiceContract()]
public interface IClientServerService
{
    [OperationContract()]
    [WebGet()]
    byte[] DownloadFiles(string fileName);
}

[ServiceBehavior(
    InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple)]
public class CacheService : ICacheService
{
    public static List<string> stringList = new List<string>();
    public static Func<int> Tid = () => Thread.CurrentThread.ManagedThreadId;

    public static Func<double> Millis = () => DateTime.Now.TimeOfDay.TotalMilliseconds;

    public string[] DisplayFiles()
    {
        string path = Directory.GetCurrentDirectory();
        string[] filesInDirectory = Directory.GetFiles(path);
        return filesInDirectory;
    }

    public byte[] DownloadFiles(string fileName)
    {
        string path = Directory.GetCurrentDirectory();
        string combined = Path.Combine(path, "CacheFiles");
        string filePath = Path.Combine(combined, fileName);
       
        if (File.Exists(filePath))
        {
            byte[] buffer = File.ReadAllBytes(filePath);
            
            stringList.Add(String.Format("User Request: File {0} at {1}, Response: Cached file {2} ", Path.GetFileName(fileName), DateTime.Now.ToString(), Path.GetFileName(fileName)));
            return buffer;
        }
        else
        {
            stringList.Add(String.Format("User Request: File {0} at {1}, Response: File {2} downloaded from the server", Path.GetFileName(fileName), DateTime.Now.ToString(), Path.GetFileName(fileName)));
            using (var wcf2 =
            new WebChannelFactory<IClientServerService>(new Uri("http://localhost:8082/hello")))
            {
                var channel2 = wcf2.CreateChannel();
                using (var scope = new OperationContextScope((IContextChannel)channel2))
                {
                    byte[] filebytes = channel2.DownloadFiles(fileName);
                    File.WriteAllBytes(filePath, filebytes);

                    return filebytes;
                }
            }
        }
    }
}

public class Form1 : System.Windows.Forms.Form
{
    // Create an instance of the ListView.
    ListView listView1 = new ListView();
    private Panel buttonPanel = new Panel();
    private Button clearCacheContentsButton = new Button();
    private Button refreshCacheContentsButton = new Button();
    private Button displayCacheLogButton = new Button();

    public Form1()
    {
        this.Load += new EventHandler(Form1_Load);
    }

    private void Form1_Load(System.Object sender, System.EventArgs e)
    {
        SetupLayout();
        SetupListView();
        PopulateListView();
    }

    private void clearCacheContentsButton_Click(object sender, EventArgs e)
    {
        string currentPath = Directory.GetCurrentDirectory();
        System.IO.DirectoryInfo di = new DirectoryInfo(currentPath + "\\CacheFiles\\");
        foreach (FileInfo file in di.EnumerateFiles())
        {
            file.Delete();
        }
        listView1.Clear(); 
    }

    private void displayCacheLogButton_Click(object sender, EventArgs e)
    {
        Form2 form2 = new Form2();
        form2.Show();
    }

    private void refreshCacheContentsButton_Click(object sender, EventArgs e)
    {
        listView1.Clear();
        listView1.Refresh();
        PopulateListView();
    }

    private void SetupLayout()
    {
        listView1.Bounds = new Rectangle(new Point(10, 10), new Size(600, 500));
        listView1.View = View.Details;
        listView1.FullRowSelect = true;
        listView1.GridLines = true;
        listView1.Sorting = SortOrder.Ascending;

        clearCacheContentsButton.Text = "Clear Cache";
        clearCacheContentsButton.Location = new Point(10, 10);
        clearCacheContentsButton.Click += new EventHandler(clearCacheContentsButton_Click);

        refreshCacheContentsButton.Text = "Refresh Cache";
        refreshCacheContentsButton.Location = new Point(100, 10);
        refreshCacheContentsButton.Click += new EventHandler(refreshCacheContentsButton_Click);

        displayCacheLogButton.Text = "Display Log";
        displayCacheLogButton.Location = new Point(200, 10);
        displayCacheLogButton.Click += new EventHandler(displayCacheLogButton_Click);

        buttonPanel.Controls.Add(clearCacheContentsButton);
        buttonPanel.Controls.Add(displayCacheLogButton);
        buttonPanel.Controls.Add(refreshCacheContentsButton);
        buttonPanel.Height = 50;
        buttonPanel.Dock = DockStyle.Bottom;

        this.Controls.Add(this.buttonPanel);
    }

    private void SetupListView()
    {
        this.Controls.Add(listView1);
    }

    private void PopulateListView()
    {
        ListViewItem filesInDirectory = new ListViewItem();
        listView1.Columns.Add("Cached Files", -2, HorizontalAlignment.Left);
        string path = Directory.GetCurrentDirectory();
        string combined = Path.Combine(path, "CacheFiles");
        string[] filesInCache = Directory.GetFiles(combined);
        
        ListViewItem listViewItem = null;
        for (int i = 0; i < filesInCache.Length; i++)
        {
            if (i % 2 == 0)
                listViewItem = new ListViewItem(new string[] { Path.GetFileName(filesInCache[i]) }, -1, Color.Empty, Color.Red, new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((System.Byte)(0))));
            else
                listViewItem = new ListViewItem(new string[] { Path.GetFileName(filesInCache[i]) }, -1, Color.Empty, Color.FromArgb(((System.Byte)(192)), ((System.Byte)(128)), ((System.Byte)(156))), null);
            this.listView1.Items.Add(listViewItem);
        }
        
    }
}

public partial class Form2 : Form
{
    ListView listView2 = new ListView();

    public Form2()
    {
        this.Load += new EventHandler(Form2_Load);
    }

    private void Form2_Load(System.Object sender, System.EventArgs e)
    {
        SetupLayout();
        SetupListView();
        PopulateListView();
    }

    private void SetupLayout()
    {
        listView2.Bounds = new Rectangle(new Point(10, 10), new Size(600, 500));
        listView2.View = View.Details;
        listView2.FullRowSelect = true;
        listView2.GridLines = true;
    }

    private void SetupListView()
    {
        this.Controls.Add(listView2);
    }

    private void PopulateListView()
    {
        listView2.Columns.Add("Cache Log", -2, HorizontalAlignment.Left);
        string[] CacheLog = CacheService.stringList.ToArray();
        
        ListViewItem listViewItem = null;
        for (int i = 0; i < CacheLog.Length; i++)
        {
            if (i % 2 == 0)
                listViewItem = new ListViewItem(new string[] { CacheLog[i] }, -1, Color.Empty, Color.Red, new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((System.Byte)(0))));
            else
                listViewItem = new ListViewItem(new string[] { CacheLog[i] }, -1, Color.Empty, Color.FromArgb(((System.Byte)(192)), ((System.Byte)(128)), ((System.Byte)(156))), null);
            this.listView2.Items.Add(listViewItem);
        }
    }
}

public class Cache
{
    public static void Main()
    {
        Uri baseAddress = new Uri("http://localhost:8081/hello");

        WebServiceHost host = null;

        try
        {
            host = new WebServiceHost(typeof(CacheService), baseAddress);
            ServiceEndpoint ep = host.AddServiceEndpoint(typeof(ICacheService), new WebHttpBinding(), "");

            host.Open();

            Console.WriteLine($"The service is ready at {baseAddress}");
            Console.WriteLine("Press <Enter> to stop the service.");
            Application.EnableVisualStyles();
            Application.Run(new Form1());
            Console.ReadLine();

            // Close the ServiceHost.
            host.Close();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"*** Exception {ex.Message}");
            host = null;

        }
        finally
        {
            if (host != null) ((IDisposable)host).Dispose();
        }
    }
}