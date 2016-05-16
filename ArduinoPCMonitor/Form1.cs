using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using GetCoreTempInfoNET;

namespace Arduino_PC_Monitor
{
    public partial class Form1 : Form
    {      
        SerialPort port;

        static CoreTempInfo CTInfo;
        GpuzWrapper gpuz;

        Graph CPUusage_Graph;
        Graph GPUusage_Graph;
        Graph CPUtemp_Graph;
        Graph GPUtemp_Graph;
        Graph Downloadspeed_Graph;
        Graph RamUsed_Graph;

        byte ticks = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.SetStyle(
                System.Windows.Forms.ControlStyles.UserPaint |
                System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                true);

            if (!IsProcessOpen("Core Temp"))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("Core Temp.exe");
                startInfo.WindowStyle = ProcessWindowStyle.Minimized;

                Process.Start(startInfo);
            }

            if (!IsProcessOpen("GPU-Z"))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("GPU-Z.0.7.1.exe");
                startInfo.Arguments = "-minimized";

                Process.Start(startInfo); 
            }

            Thread.Sleep(2000);

            CTInfo = new CoreTempInfo();
            gpuz = new GpuzWrapper();
            gpuz.Open();

            port = new SerialPort("COM3", 19200, Parity.None, 8, StopBits.One);
            button1.PerformClick();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] pts = SerialPort.GetPortNames();

            foreach (string com in SerialPort.GetPortNames())
            {
                // If COM3 exists, open or close it

                if (com == port.PortName && !port.IsOpen)
                {
                    button1.Text = "Stop";
                    timer1.Enabled = true;
                    label1.Text = "Started";
                    label1.ForeColor = Color.Green;

                    port.Open();
                }
                else
                {
                    button1.Text = "Start";
                    timer1.Enabled = false;
                    label3.Text = "0";
                    label1.Text = "Not started";
                    label1.ForeColor = Color.Black;

                    port.Write("r");
                    port.Close();
                }
            }
        }

        float totalbytes = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            CTInfo.GetData();
            byte[] statBuffer = new byte[3];

            if (port.IsOpen)
            {
                // CPU and GPU usage data

                statBuffer[0] = (byte)0;
                statBuffer[2] = (byte)Math.Round(gpuz.GetUsage(), 0);

                // RAM usage data

                int UsedRam = GetUsedRam();
                string ram = UsedRam + "Mb";

                int kbs = 0;
                string kbsec = kbs + "Kb";
                totalbytes += 0;

                // Add 128 to temperature data, can go below zero in a few rare cases

                byte cpuTemp = (byte)(Math.Round(CTInfo.GetTemp[0], 0));
                byte gpuTemp = (byte)(Math.Round(gpuz.GetTemp(), 0));

                statBuffer[0] = (byte)cpuTemp;
                statBuffer[1] = (byte)gpuTemp;

                // Update labels

                int cpuTempG = cpuTemp;
                int gpuTempG = gpuTemp;

                // Increment packets sent by 1
                label3.Text = (int.Parse(label3.Text) + 1).ToString();

                // Update GPU temp
                label5.Text = gpuTempG.ToString();

                // Send data via serial
                port.Write(statBuffer, 0, 2);
                ticks++;

                // Update graphs

                CPUusage_Graph.AddValue(cpuTempG);
                GPUusage_Graph.AddValue(gpuTempG);

                if (UsedRam > RamUsed_Graph.highest)
                {
                    RamUsed_Graph.highest = UsedRam;
                    RamUsed_Graph.mid = (int)Math.Round((double)UsedRam / 2, 0);
                }

                RamUsed_Graph.AddValue(UsedRam);

                if (kbs > Downloadspeed_Graph.highest)
                {
                    Downloadspeed_Graph.highest = kbs;
                    Downloadspeed_Graph.mid = (int)Math.Round((double)kbs / 2, 0);
                }

                Downloadspeed_Graph.AddValue(kbs);

                if (cpuTempG > CPUtemp_Graph.highest)
                {
                    CPUtemp_Graph.highest = cpuTempG;
                    CPUtemp_Graph.mid = (int)Math.Round((double)cpuTempG / 2, 0);
                }

                CPUtemp_Graph.AddValue(cpuTempG);

                if (gpuTempG > GPUtemp_Graph.highest)
                {
                    GPUtemp_Graph.highest = gpuTempG;
                    GPUtemp_Graph.mid = (int)Math.Round((double)gpuTempG / 2, 0);
                }

                GPUtemp_Graph.AddValue(gpuTempG);
            }
        }

        int GetUsedRam()
        {
            return (int)0;
            //Math.Round((new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1024f) / 1024f, 0) - (int)Math.Round(performanceCounter2.NextValue(), 0);
        }

        public bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }  

        private void Form1_Shown(object sender, EventArgs e)
        {
            CPUusage_Graph = new Graph(panel1, 0, 50, 100);
            GPUusage_Graph = new Graph(panel2, 0, 50, 100);
            CPUtemp_Graph = new Graph(panel3, 0, -1, -1);
            GPUtemp_Graph = new Graph(panel4, 0, -1, -1);
            Downloadspeed_Graph = new Graph(panel5, 0, -1, -1);
            RamUsed_Graph = new Graph(panel6, 0, -1, -1);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
