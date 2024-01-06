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
        readonly string targetFolder = @"C:\Users\malte_c8acp3s\AppData\Roaming\.minecraft\mods";
        readonly string sourceFolder1 = @"C:\Users\malte_c8acp3s\OneDrive\Skrivebord\Newest Mods";
        readonly string sourceFolder2 = @"C:\Users\malte_c8acp3s\OneDrive\Skrivebord\Skyblock Mods";
        readonly string extraModsFolder = @"C:\Users\malte_c8acp3s\OneDrive\Skrivebord\Extra Mods";
                
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

            this.refreshImage();
            this.createButtonsForExtraMods();
        }

        private void createButtonsForExtraMods()
        {
            string[] folders = Directory.GetDirectories(extraModsFolder);

            int buttons = 0;
            foreach (string folder in folders)
            {
                Button modButton = new Button();
                
                modButton.Location = new Point(30, 270+(buttons*30));
                modButton.Name = "ModButton"+buttons.ToString();
                modButton.Size = new Size(221, 25);
                modButton.TabIndex = buttons;
                modButton.Text = Path.GetFileName(folder);
                modButton.UseVisualStyleBackColor = true;
                modButton.Click += new System.EventHandler(this.ModButton_Click);
                buttons++;
                this.Controls.Add(modButton);
            }

            var size = this.Size;
            size.Height += buttons * 30;
            this.Size = size;
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

        private void refreshImage()
        {
            string imageFilename = Path.Combine(targetFolder, "image.jpg");
            
            if (!File.Exists(imageFilename))
                imageFilename = "";

            myPictureBox.ImageLocation = imageFilename;
        }
                

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            bool folder1 = isFolderContentsIdentical(targetFolder, sourceFolder1);

            string copyFrom = folder1 ? sourceFolder2 : sourceFolder1;

            install(copyFrom);

            Button button = sender as Button;
            button.Text = folder1 ? "Install Newest" : "Install Skyblock";
            this.refreshImage();

        }

        private void install(string copyFrom)
        {
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
        }

        private void ModButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            install(Path.Combine(extraModsFolder, button.Text));
            this.refreshImage();
        }
    }
}
