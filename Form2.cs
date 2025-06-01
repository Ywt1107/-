// 導入所需命名空間
using System;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit; // 本程式未用到，可刪除
using System.Windows.Forms;
//有參考Chatgpt給的框架
namespace 購物達人
{
    public partial class Form2 : Form
    {
        // 遊戲時間與金額變數
        private int countdown = 90; // 倒數初始時間（秒）
        private int remainingTimeInSeconds = 90; // 剩餘時間（秒）
        private int currentMoney; // 玩家金額
        private Random random = new Random(); // 隨機產生器

        // 計時器：倒數與 label 顯示用
        private System.Windows.Forms.Timer countdownTimer;
        private System.Windows.Forms.Timer labelTimer;

        // 商品價格 Label 與商品圖片 PictureBox 陣列
        private System.Windows.Forms.Label[] itemLabels;
        private PictureBox[] itemPictureBoxes;

        // 三張主要圖片
        private Image image1; // 特殊提示圖（例如：快一點）
        private Image image2; // 一般顯示圖（預設）
        private Image image3; // 打烊圖片

        // 控制圖片與文字狀態
        private bool isClosingTime = false;
        private bool isLabelActive = false;

        public Form2()
        {
            InitializeComponent();
            this.Load += Form2_Load;

            // 初始化 label 顯示計時器（3秒）
            labelTimer = new System.Windows.Forms.Timer();
            labelTimer.Interval = 3000;
            labelTimer.Tick += LabelTimer_Tick;

            try
            {
                // 載入三張圖片
                image1 = Image.FromFile("Images/33.png");
                image2 = Image.FromFile("Images/11.png");
                image3 = Image.FromFile("Images/12.png");
                pictureBox10.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show("載入圖片失敗：" + ex.Message);
            }

            UpdatePicture(); // 初始顯示圖片
            InitializeItemArrays(); // 初始化圖片與標籤陣列
            InitializeMoney(); // 隨機產生起始金額

            // 初始化倒數計時器
            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += CountdownTimer_Tick;
            label10.Text = countdown.ToString();
            countdownTimer.Start();

            LoadItemImages(); // 載入商品與價格
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // 設定背景圖（整張畫面）
            string imagePath = Path.Combine(Application.StartupPath, "Images4", "專櫃遊戲畫面.jpg");
            if (File.Exists(imagePath))
            {
                pictureBox11.Image = Image.FromFile(imagePath);
                pictureBox11.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox11.Dock = DockStyle.Fill;
            }
            else
            {
                MessageBox.Show("找不到圖片：" + imagePath);
            }

            // 將所有 label 設定為背景透明，並設為背景圖的子元件
            System.Windows.Forms.Label[] allLabels = new System.Windows.Forms.Label[] {
                label1, label2, label3, label4, label5, label6,
                label7, label8, label9, label10, label11, label12
            };
            foreach (var lbl in allLabels)
            {
                lbl.Parent = pictureBox11;
                lbl.BackColor = Color.Transparent;
                lbl.ForeColor = Color.Black;
            }

            // pictureBox10 是提示圖（主圖），設為背景圖的子元件並置頂
            pictureBox10.Parent = pictureBox11;
            pictureBox10.BackColor = Color.Transparent;

            // 確保提示圖與文字在最上層
            label12.BringToFront();
            pictureBox10.BringToFront();

            // 微調位置與文字格式
            pictureBox10.Left += 60;
            label12.Font = new Font(label12.Font.FontFamily, label12.Font.Size, FontStyle.Bold);
            label12.Parent = pictureBox10;
            label12.BackColor = Color.Transparent;
            label12.ForeColor = Color.Black;
            label12.Location = new Point(60, 50); // 主圖內部位置
            label12.BringToFront();

            // 初始化商品圖片與標籤位置
            ArrangePictureBoxes();
            ArrangeLabels();

            // 初始顯示歡迎語
            label12.Text = "歡迎光臨~";
            pictureBox10.Image = image2;
            isLabelActive = true;
            labelTimer.Start();
        }

        // 將商品圖分成三列三欄排列
        private void ArrangePictureBoxes()
        {
            int startX = 40;
            int startY = 38;
            int boxWidth = 100;
            int boxHeight = 100;
            int spacingX = 70;
            int spacingY = 30;

            for (int i = 0; i < itemPictureBoxes.Length; i++)
            {
                int row = i / 3;
                int col = i % 3;
                int x = startX + col * (boxWidth + spacingX);
                int y = startY + row * (boxHeight + spacingY);

                itemPictureBoxes[i].Location = new Point(x, y);
                itemPictureBoxes[i].Size = new Size(boxWidth, boxHeight);
                itemPictureBoxes[i].BringToFront();
            }
        }

        // 設定價格標籤的位置與外觀
        private void ArrangeLabels()
        {
            int labelOffsetX = 30;
            int labelOffsetY = 105;
            for (int i = 0; i < itemLabels.Length; i++)
            {
                var pb = itemPictureBoxes[i];
                itemLabels[i].Location = new Point(pb.Left + labelOffsetX, pb.Top + labelOffsetY);
                itemLabels[i].AutoSize = true;
                itemLabels[i].ForeColor = Color.Black;
                itemLabels[i].BackColor = Color.Transparent;
                itemLabels[i].Parent = pictureBox11;
                itemLabels[i].BringToFront();
            }
        }

        private void InitializeItemArrays()
        {
            itemPictureBoxes = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9 };
            itemLabels = new System.Windows.Forms.Label[] { label1, label2, label3, label4, label5, label6, label7, label8, label9 };

            // 每個商品圖加入點擊事件
            for (int i = 0; i < itemPictureBoxes.Length; i++)
            {
                int index = i;
                itemPictureBoxes[i].Click += (s, e) => PictureBox_Click(index);
            }
        }

        private void InitializeMoney()
        {
            int value = random.Next(10, 51) * 100;//1000~5000
            currentMoney = value;
            label11.Text = "$" + currentMoney.ToString();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (countdown > 0)
            {
                countdown--;
                label10.Text = countdown.ToString();
            }

            // 特定時間顯示提示
            if (countdown == 70 && !isLabelActive)
            {
                label12.Text = "想很久欸!";
                pictureBox10.Image = image1;
                isLabelActive = true;
                labelTimer.Start();
            }

            if (countdown == 50 && !isLabelActive)
            {
                label12.Text = "選快點好嗎~";
                pictureBox10.Image = image1;
                isLabelActive = true;
                labelTimer.Start();
            }

            if (countdown == 20 && !isClosingTime)
            {
                label12.Text = "即將打烊了~";
                isClosingTime = true;
                labelTimer.Start();
            }

            if (countdown <= 0)
            {
                countdown = 0;
                countdownTimer.Stop();
                CheckGameOver();
            }

            UpdatePicture();
        }

        private void CheckGameOver()
        {
            if (currentMoney == 0)
            {
                GoToFinishForm();
            }
            else if (countdown == 0)
            {
                GoToLoseForm();
            }
        }

        private void PictureBox_Click(int index)
        {
            var pb = itemPictureBoxes[index];
            if (pb.Tag is int price)
            {
                BuyItem(price);
            }
        }

        private void BuyItem(int price)
        {
            if (currentMoney >= price)
            {
                currentMoney -= price;
                label11.Text = "$" + currentMoney.ToString();
                CheckGameOver();
            }
            else
            {
                MessageBox.Show("金額不足！");
            }
        }

        private void LoadItemImages()
        {
            try
            {
                foreach (var pb in itemPictureBoxes)
                    pb.Image = null;
                foreach (var lbl in itemLabels)
                    lbl.Text = "";

                var items = new (string imagePath, int price)[]
                {
                    ("Images/LV.png", 50), ("Images/OO.png", 60), ("Images/cheap.png", 70),
                    ("Images/n1.png", 80), ("Images/n2.png", 90), ("Images/n3.png", 100),
                    ("Images/e1.png", 135), ("Images/e2.png", 165), ("Images/e3.png", 195),
                    ("Images/e4.png", 200), ("Images/m1.png", 150), ("Images/m2.png", 120),
                    ("Images/m3.png", 160), ("Images/m4.png", 180), ("Images/m5.png", 350),
                    ("Images/m6.png", 500), ("Images/m7.png", 480), ("Images/m8.png", 270),
                    ("Images/bag.png", 340)
                };

                var shuffledItems = items.OrderBy(_ => random.Next()).ToArray();
                for (int i = 0; i < itemPictureBoxes.Length; i++)
                {
                    var item = shuffledItems[i];
                    itemPictureBoxes[i].Image = Image.FromFile(item.imagePath);
                    itemPictureBoxes[i].SizeMode = PictureBoxSizeMode.StretchImage;
                    itemPictureBoxes[i].BackColor = Color.Transparent;
                    itemPictureBoxes[i].Parent = pictureBox11;
                    itemLabels[i].Text = "$" + item.price.ToString();
                    itemPictureBoxes[i].Tag = item.price;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("載入圖片時發生錯誤：" + ex.Message);
            }
        }

        private void UpdatePicture()
        {
            if (isLabelActive) return;

            if (isClosingTime)
                pictureBox10.Image = image3;
            else
                pictureBox10.Image = image2;

            label12.BringToFront();
        }

        private void LabelTimer_Tick(object sender, EventArgs e)
        {
            label12.Text = "";
            isLabelActive = false;
            isClosingTime = false;
            labelTimer.Stop();
            UpdatePicture();
        }

        private void label12_Click(object sender, EventArgs e)
        {
            label12.Text = "某些文字";
            UpdatePicture();
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            countdownTimer.Start();
        }

        private void GoToFinishForm()
        {
            countdownTimer.Stop();
            this.Hide();
            Finish finishForm = new Finish();
            finishForm.Show();
        }

        private void GoToLoseForm()
        {
            countdownTimer.Stop();
            this.Hide();
            Lose loseForm = new Lose();
            loseForm.Show();
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            // 保留空方法（可擴充）
        }
    }
}

