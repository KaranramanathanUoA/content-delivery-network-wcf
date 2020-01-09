using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

[ServiceContract()]
public interface IClientAndServerService {
    [OperationContract()]
    [WebGet()]
    string[] DisplayFiles();

    [OperationContract()] 
    [WebGet()]
    byte[] DownloadFiles(string fileName);
}

[ServiceContract()]
public interface ICacheService
{
    [OperationContract()]
    [WebGet()]
    byte[] DownloadFiles(string fileName);
}

public class Form1 : System.Windows.Forms.Form
{
    // Create an instance of the ListView.
    ListView listView1 = new ListView();
    private Panel buttonPanel = new Panel();
    private Button displayFileContentsButton = new Button();
    private Button downloadFilesButton = new Button();

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

    private void displayFileContentsButton_Click(object sender, EventArgs e)
    {
        string filename = listView1.SelectedItems[0].Text;
        string path = Directory.GetCurrentDirectory();
        string combined = Path.Combine(path, Path.GetFileName(filename));
        try
        {
            Form2 form2 = new Form2();
            form2.TextBoxValue = File.ReadAllText(combined);
            form2.Show();

        }
        catch (FileNotFoundException ex)
        {
            // Write error.
            MessageBox.Show("File not found at the location");
        }

    }

    private void downloadFilesButton_Click(object sender, EventArgs e)
    {
        string filename = listView1.SelectedItems[0].Text;

        using (var wcf =
            new WebChannelFactory<ICacheService>(new Uri("http://localhost:8081/hello")))
        {
            var channel = wcf.CreateChannel();
            byte[] filebytes = channel.DownloadFiles(filename);
            string path = Directory.GetCurrentDirectory();
            string combined = Path.Combine(path, filename);
            File.WriteAllBytes(combined, filebytes);
            MessageBox.Show("File has been downloaded!");
        }
    }

    private void SetupLayout()
    {
        listView1.Bounds = new Rectangle(new Point(10, 10), new Size(500, 200));
        listView1.View = View.Details;
        listView1.FullRowSelect = true;
        listView1.GridLines = true;
        listView1.Sorting = SortOrder.Ascending;

        displayFileContentsButton.Text = "View File";
        displayFileContentsButton.Location = new Point(10, 10);
        displayFileContentsButton.Click += new EventHandler(displayFileContentsButton_Click);

        downloadFilesButton.Text = "Download File";
        downloadFilesButton.Location = new Point(200, 10);
        downloadFilesButton.Click += new EventHandler(downloadFilesButton_Click);

        buttonPanel.Controls.Add(displayFileContentsButton);
        buttonPanel.Controls.Add(downloadFilesButton);
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
        listView1.Columns.Add("Files", -2, HorizontalAlignment.Left);
        using (var wcf =
            new WebChannelFactory<IClientAndServerService>(new Uri("http://localhost:8082/hello")))
        {
            var channel = wcf.CreateChannel();

            Console.WriteLine("Getting list of files to download");
            string[] filesInServer = channel.DisplayFiles();
            ListViewItem listViewItem = null;
            for (int i = 0; i < filesInServer.Length; i++)
            {
                if (i % 2 == 0)
                    listViewItem = new ListViewItem(new string[] { Path.GetFileName(filesInServer[i]) }, -1, Color.Empty, Color.Red, new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((System.Byte)(0))));
                else
                    listViewItem = new ListViewItem(new string[] { Path.GetFileName(filesInServer[i]) }, -1, Color.Empty, Color.FromArgb(((System.Byte)(192)), ((System.Byte)(128)), ((System.Byte)(156))), null);
                this.listView1.Items.Add(listViewItem);
            }
        }
    }
}

public partial class Form2 : Form
{
    TextBox textBox1 = new TextBox();

    public Form2()
    {
        this.Load += new EventHandler(Form2_Load);
    }

    public string TextBoxValue
    {
        get { return textBox1.Text; }
        set { textBox1.Text = value; }
    }

    private void Form2_Load(System.Object sender, System.EventArgs e)
    {
        SetupLayout();
    }

    private void SetupLayout()
    {
        this.Controls.Add(textBox1);
    }
}

    public class Client {
    public static void Main () {
                Application.EnableVisualStyles();
                Application.Run(new Form1());

            }
    }