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
using System.Diagnostics;

namespace PixProGallery
{
    public partial class Form2 : Form
    {
        public string NewImageName = "";

        int imageCount = 0;

        System.Windows.Forms.ImageList personalImageList = new ImageList();

        OpenFileDialog ofd = new OpenFileDialog()
        {
            Multiselect = true,
            ValidateNames = true,
            Filter = "All Images Files (*.png;*.jpeg;*.gif;*.jpg;*.bmp;*.tiff;*.tif)|*.png;*.jpeg;*.gif;*.jpg;*.bmp;*.tiff;*.tif" +
            "|PNG Portable Network Graphics (*.png)|*.png" +
            "|JPEG File Interchange Format (*.jpg *.jpeg *jfif)|*.jpg;*.jpeg;*.jfif" +
            "|BMP Windows Bitmap (*.bmp)|*.bmp" +
            "|TIF Tagged Imaged File Format (*.tif *.tiff)|*.tif;*.tiff" +
            "|GIF Graphics Interchange Format (*.gif)|*.gifs"
        };

        public Form2()
        {
            InitializeComponent();
            listView1.LargeImageList = personalImageList;
            this.KeyPreview = true;
            listView1.MouseClick += new MouseEventHandler(listView1_MouseClick);
        }

        // save collected data before exiting and seeing same results after
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    listView1.Clear();
        //    foreach (string reviewKey in personalImageList.Images.Keys)
        //    {
        //        //    ImageList.ImageCollection review = personalImageList.Images[reviewKey];
        //        ListViewItem item = new ListViewItem();
        //        item.SubItems.Add(personalImageList.Images[personalImageList.Images.IndexOfKey(reviewKey)]).;
        //        listView1.Items.Add(item);
        //    }
        //}

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.listView1 = personalImageList;
            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            personalImageList.ImageSize = new Size(40, 40);
            personalImageList.ColorDepth = ColorDepth.Depth16Bit;
            FileInfo fi;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in ofd.FileNames)
                {
                    try
                    {
                        fi = ImageAdding(fileName);
                    }
                    catch (Exception)
                    {

                    }

                }
            }
        }

        private FileInfo ImageAdding(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            //FileInfo fileinfo = new FileInfo(fileName);
            Console.WriteLine(fi.Name);

            ListViewItem lvi = new ListViewItem
            {
                ImageIndex = imageCount,
                Text = fi.Name,
                Tag = fi.FullName

            };

            bool found = false;
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Tag == lvi.Tag)
                {
                    found = true;
                    break;
                }
            }

                if (!found)
            {
                using (FileStream stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read))
                {
                    personalImageList.Images.Add(Image.FromStream(stream));
                }

                listView1.Items.Add(lvi);
                listView1.LargeImageList = personalImageList;

                imageCount++;
            }
            else
            {
                MessageBox.Show("There is already the same image with the same name, the image won't appear again", "Suppression", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            return fi;
        }

        //Select all images using (ctrl+A) or edit using F2 or delete using DELETE
        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
            {
                listView1.MultiSelect = true;
                foreach (ListViewItem item in listView1.Items)
                {
                    item.Selected = true;
                }
            }
            else if (e.KeyData == Keys.F2 && listView1.SelectedItems.Count > 0)
            {
                listView1.LabelEdit = true;
                listView1.SelectedItems[0].BeginEdit();
            }
            else if (e.KeyData == Keys.Delete)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    var confirmation = MessageBox.Show("Are you sure want to remove pictures from the list?", "Suppression", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirmation == DialogResult.Yes)
                    {
                        for (int i = listView1.SelectedItems.Count - 1; i >= 0; i--)
                        {
                            try
                            {
                                RemovingImage(i);
                            }
                            catch (ArgumentException)
                            {
                            }
                        }
                    }
                }
                pictureBox1.Image = null;
                listView1.Refresh();
            }
            else if (e.KeyCode == Keys.D && e.Control)
            {
                try
                {
                    this.listView1.SelectedItems.Clear();
                    pictureBox1.Image = null;
                    listView1.Refresh();
                }
                catch (Exception) { }
            }
        }
        // Remove files selected by mouse
        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count > 0)
            {
                var confirmation = MessageBox.Show("Are you sure want to remove pictures from the list?", "Suppression", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmation == DialogResult.Yes)
                {
                    for (int i = listView1.SelectedItems.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            RemovingImage(i);
                        }
                        catch (ArgumentException)
                        {
                        }
                    }
                }
            }
            pictureBox1.Image = null;
            listView1.Refresh();
        }



        private void RemovingImage(int i)
        {

            ListViewItem itm = listView1.SelectedItems[i];
            int removeAt = itm.Index;
            Console.WriteLine(removeAt);

            // for removing original file.
            //My.Computer.FileSystem.DeleteFile(ListView1.SelectedItems(0).Tag.ToString, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)

            personalImageList.Images.RemoveByKey(listView1.SelectedItems[i].ImageKey);

            listView1.Items.Remove(listView1.SelectedItems[i]);

            personalImageList = listView1.LargeImageList;

            for (int j = listView1.Items.Count; j >= removeAt; j--)
            {
                listView1.Items[j].ImageIndex--;
            }

            imageCount--;
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem theClickedOne = listView1.GetItemAt(e.X, e.Y);

            if (theClickedOne != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void OnMenuItemClick(object sender, EventArgs e)
        {
            listView1.SelectedItems[0].BeginEdit();
        }
        //rename
        private void listView1_AfterLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e)
        {
            if (e.Label == null)
                return;
            NewImageName = Convert.ToString(e.Label);

            ListViewItem item1 = listView1.SelectedItems[0];

            FileInfo fileInfo = new FileInfo(item1.Tag.ToString());
            fileInfo.MoveTo(fileInfo.Directory.FullName + "\\" + NewImageName);
            listView1.Items[item1.Index].Text = NewImageName;
            listView1.Items[item1.Index].Tag = fileInfo.Directory.FullName + "\\" + NewImageName;


        }

        // Display selected image
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Bitmap bm;
            pictureBox1.InitialImage = null;

            if (listView1.SelectedItems.Count == 1)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                ListViewItem itm = listView1.SelectedItems[0];
                int removeAt = itm.Index;
                //Console.WriteLine(removeAt);

                string s = listView1.SelectedItems[0].Tag.ToString();
                listView1.Refresh();

                using (FileStream stream = new FileStream(s, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    var memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
                    try
                    {
                        bm = new Bitmap(memoryStream);
                        pictureBox1.Image = bm;
                    }
                    catch (Exception)
                    {
                        pictureBox1.Dispose();
                    }
                }
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void contextMenuStrip1_Opening_1(object sender, CancelEventArgs e)
        {

        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        // Searcing [recursevly] folder
        private void button1_Click_1(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    String searchFolder = @"" + fbd.SelectedPath;
                    var filters = new String[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp" };
                    var files = GetFilesFrom(searchFolder, filters, checkBox1.Checked);

                    personalImageList.ImageSize = new Size(40, 40);
                    personalImageList.ColorDepth = ColorDepth.Depth16Bit;
                    FileInfo fi;

                    foreach (string fileName in files)
                    {
                        try
                        {
                            fi = ImageAdding(fileName);
                        }
                        catch (Exception)
                        {
                            listView1.Refresh();
                        }
                    }
                }
            }
        }

        // Popout Rename
        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.LabelEdit = true;
            listView1.SelectedItems[0].BeginEdit();
        }

        // select all popup
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.MultiSelect = true;
            foreach (ListViewItem item in listView1.Items)
            {
                item.Selected = true;
            }
        }

        public static List<String> GetFilesFrom(String searchFolder, String[] filters, bool isRecursive)
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                try
                {
                    filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));
                }
                catch (UnauthorizedAccessException) { }
            }
            return filesFound;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
