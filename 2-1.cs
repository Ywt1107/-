using System;
using System.Net;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
//有參考Chatgpt給的框架
namespace 購物達人
{
    public partial class _2_1 : Form
    {
        public _2_1()
        {
            InitializeComponent();
            this.Load += _2_1_Load;
            this.Resize += _2_1_Resize;
        }

        private void _2_1_Load(object sender, EventArgs e)
        {
            LoadImageFromGitHub();

            PositionButtons();

            // 設定按鈕文字
            button1.Text = "簡單";
            button2.Text = "普通";
            button3.Text = "困難";
            button4.Text = "返回";

            // 設定按鈕文字為粗體
            button1.Font = new Font(button1.Font.FontFamily, 15, FontStyle.Bold); // 簡單
            button2.Font = new Font(button2.Font.FontFamily, 15, FontStyle.Bold); // 普通
            button3.Font = new Font(button3.Font.FontFamily, 15, FontStyle.Bold); // 困難
            button4.Font = new Font(button4.Font.FontFamily, 10, FontStyle.Bold); // 返回
        }
        private async void LoadImageFromGitHub()
        {
            string imageUrl = "https://raw.githubusercontent.com/Ywt1107/-/master/Images4/主題難易.jpg";

            try
            {
                using (WebClient client = new WebClient())
                {
                    // 從 GitHub 下載圖片成位元組資料
                    byte[] imageBytes = await client.DownloadDataTaskAsync(imageUrl);

                    // 將位元組轉為圖片
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        pictureBox1.Image = Image.FromStream(ms);
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox1.Dock = DockStyle.Fill;
                        pictureBox1.SendToBack(); // 設為背景
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("圖片載入失敗：" + ex.Message);
            }
        }
        private void _2_1_Resize(object sender, EventArgs e)
        {
            PositionButtons();
        }

        private void PositionButtons()
        {
            int spacing = Math.Min(80, Math.Max(40, this.ClientSize.Width / 20));
            int buttonWidth = Math.Max(180, this.ClientSize.Width / 6);
            int buttonHeight = Math.Max(50, this.ClientSize.Height / 10);

            // 按鈕4縮小
            int button4Width = Math.Max(80, this.ClientSize.Width / 6);
            int button4Height = Math.Max(30, this.ClientSize.Height / 15);
            button4.Size = new Size(button4Width, button4Height);
            int button4X = 10;
            int button4Y = this.ClientSize.Height - button4Height - 10;
            button4.Location = new Point(button4X, button4Y);

            // 按鈕1、2、3尺寸與位置
            button1.Size = new Size(buttonWidth, buttonHeight);
            button2.Size = new Size(buttonWidth, buttonHeight);
            button3.Size = new Size(buttonWidth, buttonHeight);

            int totalWidth = buttonWidth * 3 + spacing * 2;
            int startX = (this.ClientSize.Width - totalWidth) / 2;
            int centerY = this.ClientSize.Height / 2;

            button1.Location = new Point(startX, centerY - buttonHeight / 2);
            button2.Location = new Point(startX + buttonWidth + spacing, centerY - buttonHeight / 2);
            button3.Location = new Point(startX + 2 * (buttonWidth + spacing), centerY - buttonHeight / 2);

            // 設定圓角
            SetButtonRadius(button1, 20);
            SetButtonRadius(button2, 20);
            SetButtonRadius(button3, 20);
            // 按鈕4不用圓角或也可以設
            SetButtonRadius(button4, 10);
        }

        // 設定圓角的輔助方法
        private void SetButtonRadius(Button btn, int radius)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
            path.AddLine(radius, 0, btn.Width - radius, 0);
            path.AddArc(new Rectangle(btn.Width - radius, 0, radius, radius), -90, 90);
            path.AddLine(btn.Width, radius, btn.Width, btn.Height - radius);
            path.AddArc(new Rectangle(btn.Width - radius, btn.Height - radius, radius, radius), 0, 90);
            path.AddLine(btn.Width - radius, btn.Height, radius, btn.Height);
            path.AddArc(new Rectangle(0, btn.Height - radius, radius, radius), 90, 90);
            path.CloseFigure();
            btn.Region = new Region(path);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Form4 gameForm = new Form4();
            gameForm.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form5 gameForm = new Form5();
            gameForm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form6 gameForm = new Form6();
            gameForm.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            choose1 backForm = new choose1();
            backForm.Show();
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // 如果要處理圖片點擊事件，寫這裡
        }
    }
}