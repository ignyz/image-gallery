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

namespace PixProGallery
{
    public partial class Form1 : Form
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

        FileInfo fi;
        Bitmap bm;
        public Form1()
        {
            InitializeComponent();
            listView1.SmallImageList = personalImageList;
            this.KeyPreview = true;
            listView1.MouseClick += new MouseEventHandler(listView1_MouseClick);
        }
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
            personalImageList.ImageSize = new Size(140, 140);
            personalImageList.ColorDepth = ColorDepth.Depth32Bit;
            //personalImageList

            if (ofd.ShowDialog() == DialogResult.OK)
            {

                foreach (string fileName in ofd.FileNames)
                {

                    fi = new FileInfo(fileName);
                    FileInfo fileinfo = new FileInfo(fileName);
                    using (FileStream stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read))
                    {
                        personalImageListLocation.Add(fileName);
                        personalImageList.Images.Add(Image.FromStream(stream));
                        string a = imageCount.ToString(fi.FullName);
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
            }
        }
        //Select all images using (ctrl+A)
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    item.Selected = true;
                    listView1.Focus();

                }
            }
        }
        /// <summary>
        /// Here i have create dynamically shortcut menu Contextmenu
        /// Add New menuItem "Rename"
        /// Create OnMenuItemClick event on MenuItem Click
        ///
        /// </summary>
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
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                int s = listView1.SelectedItems[0].ImageIndex;

                try
                {
                    bm = new Bitmap(personalImageListLocation[s]);//listView1.Items[s].Text);
                    pictureBox1.Image = bm;
                }
                catch (ArgumentException)
                {

                }
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        
        }

        private void contextMenuStrip1_Opening_1(object sender, CancelEventArgs e)
        {

        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContextMenu cm = new ContextMenu();

            listView1.ContextMenu = cm;
            var mi = new MenuItem("Rename");

            mi.MenuItems.Add(mi);
            mi.Click += OnMenuItemClick;
            cm.MenuItems.Add(mi);
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
                            personalImageList.Images.RemoveAt(itm.Index);
                            personalImageListLocation.RemoveAt(itm.Index);
                            listView1.Items[itm.Index].Remove();
                            imageCount--;
                        }
                        catch (ArgumentException )
                        {
                        }
                    }
                }

            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtReview.Text))
            //    return;
            ////First we should set Model data
            //Model.AddReview("moive1", txtName.Text, txtReview.Text);
            //LoadListView();
        }
    }
}
