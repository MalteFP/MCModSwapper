using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SystemTrayApp
{
    public partial class AppWindow : Form
    {
        public AppWindow()
        {
            InitializeComponent();
            this.CenterToScreen();

            // To provide your own custom icon image, go to:
            //   1. Project > Properties... > Resources
            //   2. Change the resource filter to icons
            //   3. Remove the Default resource and add your own
            //   4. Modify the next line to Properties.Resources.<YourResource>
            this.Icon = Properties.Resources.Default;
            this.SystemTrayIcon.Icon = Properties.Resources.Default;

            // Change the Text property to the name of your application
            this.SystemTrayIcon.Text = "System Tray App";
            this.SystemTrayIcon.Visible = true;

            // Modify the right-click menu of your system tray icon here
            ContextMenu menu = new ContextMenu();
            menu.MenuItems.Add("Exit", ContextMenuExit);
            this.SystemTrayIcon.ContextMenu = menu;

            this.Resize += WindowResize;
            this.FormClosing += WindowClosing;
        }
        
        private void SystemTrayIconDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void ContextMenuExit(object sender, EventArgs e)
        {
            this.SystemTrayIcon.Visible = false;
            Application.Exit();
            Environment.Exit(0);
        }

        private void WindowResize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void WindowClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }


        private bool isFolderContentsIdentical(string testFolder, string baselineFolder)
        {
            string[] filePaths = Directory.GetFiles(baselineFolder);

            foreach (string filePath in filePaths)
            {
                string filename = Path.GetFileName(filePath);
                string destinationFilename = Path.Combine(testFolder, filename);
                if (!File.Exists(destinationFilename))
                    return false;
            }

            return true;
        }

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            string targetFolder = @"C:\Users\malte_c8acp3s\AppData\Roaming\.minecraft\mods";
            string sourceFolder1 = @"C:\Users\malte_c8acp3s\OneDrive\Skrivebord\Newest Mods";
            string sourceFolder2 = @"C:\Users\malte_c8acp3s\OneDrive\Skrivebord\Skyblock Mods";

            bool folder1 = isFolderContentsIdentical(targetFolder, sourceFolder1);

            string copyFrom = folder1 ? sourceFolder2 : sourceFolder1;

            // Delete all files
            string[] filePaths = Directory.GetFiles(targetFolder);
            foreach (string filePath in filePaths)
                File.Delete(filePath);

            // copy files
            filePaths = Directory.GetFiles(copyFrom);
            foreach (string filePath in filePaths)
            {
                string filename = Path.GetFileName(filePath);
                string destinationFilename = Path.Combine(targetFolder, filename);
                File.Copy(filePath, destinationFilename);
            }

            Button button = sender as Button;
            button.Text = folder1 ? "Install Newest" : "Install Skyblock";

        }
    }
}
