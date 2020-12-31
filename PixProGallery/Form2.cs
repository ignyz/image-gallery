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

        List<string> personalImageListLocation = new List<string>();
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
            listView1.SmallImageList = personalImageList;
            this.KeyPreview = true;
            listView1.MouseClick += new MouseEventHandler(listView1_MouseClick);


        }

        // Tried to save collected data before exiting and seeing same results after

        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    listView1.Clear();
        //    foreach (string reviewKey in Model.ReviewData.Keys)
        //    {
        //        Review review = Model.ReviewData[reviewKey];
        //        ListViewItem item = new ListViewItem(review.reviewerName);
        //        item.SubItems.Add(review.review);
        //        listView1.Items.Add(item);
        //    }
        //}

        //private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    Properties.Settings.Default.listView1 = textBox1.Text;
        //    Properties.Settings.Default.Save();
        //}

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

                        fi = new FileInfo(fileName);
                        FileInfo fileinfo = new FileInfo(fileName);
                        using (FileStream stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read))
                        {
                            personalImageListLocation.Add(fileName);
                            personalImageList.Images.Add(Image.FromStream(stream));
                            //imageCount++;
                        }
                        listView1.LargeImageList = personalImageList;

                        listView1.Items.Add(new ListViewItem
                        {
                            ImageIndex = imageCount,
                            Text = fi.Name,
                            Tag = fi.FullName
                        });

                        imageCount++;
                    }
                    catch (Exception)
                    {
                        listView1.Dispose();
                    }
                   
                }
            }
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
                int removeAt = 0;
                if (listView1.SelectedItems.Count > 0)
                {
                    var confirmation = MessageBox.Show("Are you sure want to remove pictures from the list?", "Suppression", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirmation == DialogResult.Yes)
                    {
                        for (int i = listView1.SelectedItems.Count - 1; i >= 0; i--)
                        {
                            try
                            {
                                ListViewItem itm = listView1.SelectedItems[i];
                                removeAt = itm.Index;

                                //personalImageListLocation.RemoveAt(removeAt);
                                //personalImageList.Images.RemoveAt(removeAt);
                                listView1.Items[itm.Index].Remove();
                                imageCount--;
                              
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
            fileInfo.MoveTo(fileInfo.Directory.FullName + "\\" + NewImageName + fileInfo.Extension);
            listView1.Items[imageCount].Text = NewImageName;

        }

        // Display selected image
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            listView1.Refresh();

            Bitmap bm;
            pictureBox1.InitialImage = null;

            if (listView1.SelectedItems.Count == 1)
            {

                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                int s = listView1.SelectedItems[0].ImageIndex;

                using (FileStream stream = new FileStream(personalImageListLocation[s], FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    var memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
                    try
                    {

                        bm = new Bitmap(memoryStream);//listView1.Items[s].Text);
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

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        // Rename
        private void contextMenuStrip1_Opening_1(object sender, CancelEventArgs e)
        {

        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        // Remove
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
                            ListViewItem itm = listView1.SelectedItems[i];
                            //personalImageListLocation.RemoveAt(itm.Index);
                            listView1.Items[itm.Index].Remove();
                            imageCount--;
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
        // Save changes before exiting
        private void button1_Click_1(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    //String searchFolder = @"C:\Users\Ignas\OneDrive - Kaunas University of Technology\PixPro\Images";
                    String searchFolder = @"" + fbd.SelectedPath;//@"C:\Users\Ignas\Pictures";
                    var filters = new String[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp" };
                    var files = GetFilesFrom(searchFolder, filters, checkBox1.Checked);


                    personalImageList.ImageSize = new Size(40, 40);
                    personalImageList.ColorDepth = ColorDepth.Depth32Bit;
                    FileInfo fi;



                    foreach (string fileName in files)
                    {
                        try
                        {
                            fi = new FileInfo(fileName);
                            FileInfo fileinfo = new FileInfo(fileName);
                            using (FileStream stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read))
                            {
                                personalImageListLocation.Add(fileName);
                                personalImageList.Images.Add(Image.FromStream(stream));
                                //imageCount++;
                            }
                            listView1.LargeImageList = personalImageList;

                            listView1.Items.Add(new ListViewItem
                            {
                                ImageIndex = imageCount,
                                Text = fi.Name,
                                Tag = fi.FullName
                            });

                            imageCount++;
                        }
                        catch (Exception)
                        {
                            listView1.Dispose();
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
