using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Windows.Forms;

namespace DriveMonitor
{
    public partial class Form1 : Form
    {
        StreamWriter myWriter;
        FileInfo myFile;

        DriveInfo[] myDrives = null;
        string[,] myRegisteredDrives = new string[5, 2];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            register_drive();

        }

        public void register_drive()
        {
            try
            {
                myDrives = DriveInfo.GetDrives();

                for (int i = 0; i < myDrives.Length; i++)
                {

                    if ((myRegisteredDrives[i, 0] == null || myRegisteredDrives[i, 0] == "") && (myDrives[i].DriveType.ToString().ToUpper() == "REMOVABLE" || myDrives[i].DriveType.ToString().ToUpper() == "CD-ROM"))
                    {
                        myRegisteredDrives[i, 0] = myDrives[i].RootDirectory.FullName;
                        myRegisteredDrives[i, 1] = $"{myDrives[i].Name} {myDrives[i].DriveType}|Memory Available: {(Convert.ToDouble(myDrives[i].AvailableFreeSpace) / 1000000).ToString("f0")} MB|User: {Environment.UserName}|Time: {DateTime.Now}";
                        Record_Drive($"Drive Inserted {myRegisteredDrives[i, 1]}");
                    }
                }
            }
            catch
            {

            }
            finally
            {
                myDrives = null;
            }
        }

        public void update_drive_status()
        {
            try
            {

                myDrives = DriveInfo.GetDrives();

                for (int i = 0; i < myRegisteredDrives.GetLength(0); i++)
                {
                    if (myRegisteredDrives[i, 0] != null && myRegisteredDrives[i, 0] != "")
                    {
                        if (!Directory.Exists(myRegisteredDrives[i, 0]))
                        {
                            myRegisteredDrives[i, 0] = "";

                            if(myRegisteredDrives[i, 1] != null && myRegisteredDrives[i, 1] != "")
                            {
                                Record_Drive($"Drive Removed {myRegisteredDrives[i, 1]}");
                            }                            

                        }
                        else if (myDrives[i].RootDirectory.FullName == myRegisteredDrives[i, 0])
                        {
                            myRegisteredDrives[i, 0] = myDrives[i].RootDirectory.FullName;
                            myRegisteredDrives[i, 1] = $"{myDrives[i].Name} {myDrives[i].DriveType}|Memory Available: {(Convert.ToDouble(myDrives[i].AvailableFreeSpace) / 1000000).ToString("f0")} MB|User: {Environment.UserName}|Time: {DateTime.Now}";
                        }

                    }
                }
            }
            catch
            {

            }
            finally
            {
                myDrives = null;
            }

        }

        public void Record_Drive(string myDrive)
        {
            try
            {
                myWriter = new StreamWriter("Drive Activity Log.txt", true);

                myWriter.WriteLine($"{myDrive}");

            }
            catch (Exception ex)
            {
                myWriter.Close();
                MessageBox.Show(ex.Message, "Error - Logging Drive Activity");
            }
            finally
            {
                myDrives = null;
                myWriter.Close();
            }
        }

        public void Watch_Drives()
        {
            try
            {
                if (myRegisteredDrives[0, 0] != null)
                {
                    if (myRegisteredDrives[0, 0] != "")
                    {
                        fileSystemWatcher1.Path = myRegisteredDrives[0, 0];
                        fileSystemWatcher1.EnableRaisingEvents = true;
                    }
                    else
                    {
                        fileSystemWatcher1.EnableRaisingEvents = false;
                    }
                }

                if (myRegisteredDrives[1, 0] != null)
                {
                    if (myRegisteredDrives[1, 0] != "")
                    {
                        fileSystemWatcher2.Path = myRegisteredDrives[1, 0];
                        fileSystemWatcher2.EnableRaisingEvents = true;
                    }
                    else
                    {
                        fileSystemWatcher2.EnableRaisingEvents = false;
                    }
                }

                if (myRegisteredDrives[2, 0] != null)
                {
                    if (myRegisteredDrives[2, 0] != "")
                    {
                        fileSystemWatcher3.Path = myRegisteredDrives[2, 0];
                        fileSystemWatcher3.EnableRaisingEvents = true;
                    }
                    else
                    {
                        fileSystemWatcher3.EnableRaisingEvents = false;
                    }
                }

                if (myRegisteredDrives[3, 0] != null)
                {
                    if (myRegisteredDrives[3, 0] != "")
                    {
                        fileSystemWatcher4.Path = myRegisteredDrives[3, 0];
                        fileSystemWatcher4.EnableRaisingEvents = true;
                    }
                    else
                    {
                        fileSystemWatcher4.EnableRaisingEvents = false;
                    }
                }

                if (myRegisteredDrives[4, 0] != null)
                {
                    if (myRegisteredDrives[4, 0] != "")
                    {
                        fileSystemWatcher5.Path = myRegisteredDrives[4, 0];
                        fileSystemWatcher5.EnableRaisingEvents = true;
                    }
                    else
                    {
                        fileSystemWatcher5.EnableRaisingEvents = false;
                    }
                }

            }
            catch
            {

            }
            finally
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            register_drive();
            update_drive_status();
            Watch_Drives();
        }
       public void Record_Drive_Rename_Activity(RenamedEventArgs e)
        {
            try
            {
                myFile = new FileInfo(e.FullPath);
                myWriter = new StreamWriter("Drive Activity Log.txt", true);

                if (myFile.Attributes.ToString().Contains("Directory"))
                {
                    myWriter.WriteLine($"Folder {e.ChangeType}|Actioned by: {Environment.UserName}|Time: {DateTime.Now}|Old Folder Name: {e.OldFullPath.Split('\\')[e.FullPath.Split('\\').Length - 1]}|New Folder Name: {e.FullPath.Split('\\')[e.FullPath.Split('\\').Length - 1]}|Folder Path: {e.FullPath}");
                }
                else
                {
                    myWriter.WriteLine($"File {e.ChangeType}|Actioned by: {Environment.UserName}|Time: {DateTime.Now}|Old File Name: {e.OldFullPath.Split('\\')[e.FullPath.Split('\\').Length - 1]}|New File Name: {e.FullPath.Split('\\')[e.FullPath.Split('\\').Length - 1]}|File Path: {e.FullPath}");
                }
            }
            catch (Exception ex)
            {
                myWriter.Close();
                myFile = null;
                MessageBox.Show(ex.Message, "Error - Logging drive activity");
            }
            finally
            {
                myWriter.Close();
                myFile = null;
                e = null;
            }
        }

        public void Record_Drive_Activity(FileSystemEventArgs e)
        {
            try
            {

                myFile = new FileInfo(e.FullPath);
                myWriter = new StreamWriter("Drive Activity Log.txt", true);

                if (myFile.Attributes.ToString().Contains("Directory"))
                {
                    myWriter.WriteLine($"Folder {e.ChangeType}|Actioned by: {Environment.UserName}|Time: {DateTime.Now}|Folder Name: {e.FullPath.Split('\\')[e.FullPath.Split('\\').Length - 1]}|Folder Path: {e.FullPath}");
                }
                else
                {
                    myWriter.WriteLine($"File {e.ChangeType}|Actioned by: {Environment.UserName}|Time: {DateTime.Now}|File Name: {e.FullPath.Split('\\')[e.FullPath.Split('\\').Length - 1]}|File Path: {e.FullPath}");
                }

            }
            catch (Exception ex)
            {
                myWriter.Close();
                myFile = null;
                MessageBox.Show(ex.Message, "Error - Loggin drive activity");
            }
            finally
            {
                myWriter.Close();
                myFile = null;
                e = null;
            }
        }

        private void fileSystemWatcher1_Created(object sender, FileSystemEventArgs e)
        {
            Record_Drive_Activity(e);
        }

        private void fileSystemWatcher1_Deleted(object sender, FileSystemEventArgs e)
        {
            Record_Drive_Activity(e);
        }

        private void fileSystemWatcher1_Renamed(object sender, RenamedEventArgs e)
        {
            Record_Drive_Rename_Activity(e);
        }

        private void fileSystemWatcher2_Created(object sender, FileSystemEventArgs e)
        {
            Record_Drive_Activity(e);
        }

        private void fileSystemWatcher2_Deleted(object sender, FileSystemEventArgs e)
        {
            Record_Drive_Activity(e);
        }

        private void fileSystemWatcher2_Renamed(object sender, RenamedEventArgs e)
        {
            Record_Drive_Rename_Activity(e);
        }

        private void fileSystemWatcher3_Created(object sender, FileSystemEventArgs e)
        {
            Record_Drive_Activity(e);
        }

        private void fileSystemWatcher3_Deleted(object sender, FileSystemEventArgs e)
        {
            Record_Drive_Activity(e);
        }

        private void fileSystemWatcher3_Renamed(object sender, RenamedEventArgs e)
        {
            Record_Drive_Rename_Activity(e);
        }

        private void fileSystemWatcher4_Created(object sender, FileSystemEventArgs e)
        {
            Record_Drive_Activity(e);
        }

        private void fileSystemWatcher4_Deleted(object sender, FileSystemEventArgs e)
        {
            Record_Drive_Activity(e);
        }

        private void fileSystemWatcher4_Renamed(object sender, RenamedEventArgs e)
        {
            Record_Drive_Rename_Activity(e);
        }

        private void fileSystemWatcher5_Created(object sender, FileSystemEventArgs e)
        {
            Record_Drive_Activity(e);
        }

        private void fileSystemWatcher5_Deleted(object sender, FileSystemEventArgs e)
        {
            Record_Drive_Activity(e);
        }

        private void fileSystemWatcher5_Renamed(object sender, RenamedEventArgs e)
        {
            Record_Drive_Rename_Activity(e);
        }
    }
}
