using System;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SharpPortScanner
{
    public class PortScannerForm : Form
    {
        private TextBox txtHost;
        private TextBox txtStartPort;
        private TextBox txtEndPort;
        private Button btnScan;
        private ListBox lstResults;
        private Label lblStatus;

        public PortScannerForm()
        {
            this.Text = "SharpPortScanner - TCP Port Scanner";
            this.Size = new Size(600, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblHost = new Label { Text = "Target IP/Host:", Location = new Point(20, 20), AutoSize = true };
            txtHost = new TextBox { Location = new Point(130, 18), Width = 200, Text = "127.0.0.1" };

            Label lblStart = new Label { Text = "Start Port:", Location = new Point(20, 60), AutoSize = true };
            txtStartPort = new TextBox { Location = new Point(130, 58), Width = 100, Text = "1" };

            Label lblEnd = new Label { Text = "End Port:", Location = new Point(250, 60), AutoSize = true };
            txtEndPort = new TextBox { Location = new Point(330, 58), Width = 100, Text = "1024" };

            btnScan = new Button { Text = "Start Scan", Location = new Point(450, 55), Width = 100 };
            btnScan.Click += BtnScan_Click;

            lstResults = new ListBox { Location = new Point(20, 100), Width = 530, Height = 300, Font = new Font("Consolas", 9) };

            lblStatus = new Label { Text = "Status: Idle", Location = new Point(20, 420), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

            this.Controls.Add(lblHost);
            this.Controls.Add(txtHost);
            this.Controls.Add(lblStart);
            this.Controls.Add(txtStartPort);
            this.Controls.Add(lblEnd);
            this.Controls.Add(txtEndPort);
            this.Controls.Add(btnScan);
            this.Controls.Add(lstResults);
            this.Controls.Add(lblStatus);
        }

        private async void BtnScan_Click(object sender, EventArgs e)
        {
            string host = txtHost.Text;
            if (!int.TryParse(txtStartPort.Text, out int startPort) || !int.TryParse(txtEndPort.Text, out int endPort))
            {
                MessageBox.Show("Please enter valid port numbers.");
                return;
            }

            if (startPort < 1 || endPort > 65535 || startPort > endPort)
            {
                MessageBox.Show("Port range must be between 1 and 65535.");
                return;
            }

            btnScan.Enabled = false;
            lstResults.Items.Clear();
            lblStatus.Text = $"Scanning {host} from port {startPort} to {endPort}...";

            await Task.Run(() => ScanPorts(host, startPort, endPort));

            lblStatus.Text = "Scan complete.";
            btnScan.Enabled = true;
        }

        private void ScanPorts(string host, int startPort, int endPort)
        {
            int total = endPort - startPort + 1;
            int scanned = 0;
            int concurrent = 100;
            SemaphoreSlim throttle = new SemaphoreSlim(concurrent);

            List<Task> tasks = new List<Task>();

            for (int port = startPort; port <= endPort; port++)
            {
                throttle.Wait();
                int currentPort = port;
                var task = Task.Run(async () =>
                {
                    bool open = await IsPortOpen(host, currentPort, 100);
                    this.Invoke(new Action(() =>
                    {
                        string status = open ? "[OPEN]   " : "[CLOSED] ";
                        lstResults.Items.Add($"{status} Port {currentPort}");
                        scanned++;
                        lblStatus.Text = $"Scanned {scanned}/{total} ports...";
                    }));
                    throttle.Release();
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }

        private async Task<bool> IsPortOpen(string host, int port, int timeout)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    var connectTask = client.ConnectAsync(host, port);
                    var timeoutTask = Task.Delay(timeout);
                    var completed = await Task.WhenAny(connectTask, timeoutTask);
                    return completed == connectTask && client.Connected;
                }
            }
            catch
            {
                return false;
            }
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new PortScannerForm());
        }
    }
}
