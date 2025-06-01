using System;                        // 提供基本的 .NET 功能（如數學運算、日期處理等）
using System.Drawing;                // 提供圖形功能（如圖片、顏色等）
using System.Linq;                   // 提供 LINQ 查詢語法支援
using System.Reflection.Emit;        // 用於動態建立方法或型別（本程式未使用，可刪除）
using System.Windows.Forms;          // 提供 Windows Form 應用程式 UI 元件支援
//有參考Chatgpt給的框架
namespace 購物達人 // 命名空間：購物達人
{
    public partial class Form9 : Form // Form9 繼承自 Windows Form 表單
    {
        // ====== 全域變數區 ======
        private int countdown = 120; // 遊戲倒數秒數初始值
        private int remainingTimeInSeconds = 120; // 剩餘時間（可與 countdown 整合）
        private int currentMoney; // 玩家當前金額
        private Random random = new Random(); // 用於亂數選商品與金額

        private System.Windows.Forms.Timer countdownTimer; // 倒數計時器
        private System.Windows.Forms.Timer labelTimer; // 控制 label12 文字顯示時間的計時器

        private System.Windows.Forms.Label[] itemLabels; // 存放每個商品價格的 label 陣列
        private PictureBox[] itemPictureBoxes; // 存放每個商品圖片的 PictureBox 陣列

        // 主圖片顯示用的三張圖（歡迎圖、催促圖、打烊圖）
        private Image image1; // 圖片1：催促圖
        private Image image2; // 圖片2：一般狀態圖
        private Image image3; // 圖片3：打烊圖

        private bool isClosingTime = false; // 是否進入打烊階段
        private bool isLabelActive = false; // 是否正在顯示 label12 文字

        public Form9()
        {
            InitializeComponent(); // 初始化表單元件
            this.Load += Form9_Load; // 註冊載入事件

            // 初始化 label 計時器，顯示 label12 文字 3 秒
            labelTimer = new System.Windows.Forms.Timer();
            labelTimer.Interval = 3000;
            labelTimer.Tick += LabelTimer_Tick;

            try
            {
                // 載入三張主要圖片
                image1 = Image.FromFile("Images3/23.png");
                image2 = Image.FromFile("Images3/31.png");
                image3 = Image.FromFile("Images3/32.png");
                pictureBox10.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show("載入圖片失敗：" + ex.Message);
            }

            UpdatePicture();         // 初始主圖片為 image2
            InitializeItemArrays();  // 初始化圖片與標籤陣列
            InitializeMoney();       // 隨機產生起始金額

            // 初始化倒數計時器
            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += CountdownTimer_Tick;
            label10.Text = countdown.ToString(); // 顯示初始時間
            countdownTimer.Start();            // 開始倒數

            LoadItemImages(); // 載入隨機商品與價格
        }

        private void Form9_Load(object sender, EventArgs e)
        {
            // 設定背景圖
            string imagePath = Path.Combine(Application.StartupPath, "Images4", "玩具遊戲畫面.jpg");
            if (File.Exists(imagePath))
            {
                pictureBox11.Image = Image.FromFile(imagePath);
                pictureBox11.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox11.Dock = DockStyle.Fill; // 填滿整個畫面
            }
            else
            {
                MessageBox.Show("找不到圖片：" + imagePath);
            }

            // 將 Label 設為背景透明並指定為 pictureBox11 的子物件
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

            // 設定主圖片與提示文字的層級與樣式
            pictureBox10.Parent = pictureBox11;
            pictureBox10.BackColor = Color.Transparent;
            pictureBox10.BringToFront();
            label12.BringToFront();

            label10.ForeColor = Color.Black;
            label11.ForeColor = Color.Black;

            pictureBox10.Left += 60; // 微調位置
            label12.Font = new Font(label12.Font.FontFamily, label12.Font.Size, FontStyle.Bold);
            label12.Parent = pictureBox10;
            label12.BackColor = Color.Transparent;
            label12.ForeColor = Color.Black;
            label12.Location = new Point(60, 40);
            label12.BringToFront();

            ArrangePictureBoxes(); // 排列商品圖片
            ArrangeLabels();       // 排列價格標籤

            // 初始訊息與圖片
            label12.Text = "歡迎光臨~";
            pictureBox10.Image = image2;
            isLabelActive = true;
            labelTimer.Start();
        }

        private void ArrangePictureBoxes()
        {
            // 商品圖片排列（3列3行）
            int startX = 40, startY = 38;
            int boxWidth = 100, boxHeight = 100;
            int spacingX = 70, spacingY = 30;

            for (int i = 0; i < itemPictureBoxes.Length; i++)
            {
                int row = i / 3, col = i % 3;
                int x = startX + col * (boxWidth + spacingX);
                int y = startY + row * (boxHeight + spacingY);

                itemPictureBoxes[i].Location = new Point(x, y);
                itemPictureBoxes[i].Size = new Size(boxWidth, boxHeight);
                itemPictureBoxes[i].BringToFront();
            }
        }

        private void ArrangeLabels()
        {
            // 價格標籤位置與屬性設定
            int offsetX = 30, offsetY = 105;

            for (int i = 0; i < itemLabels.Length; i++)
            {
                var pb = itemPictureBoxes[i];
                itemLabels[i].Location = new Point(pb.Left + offsetX, pb.Top + offsetY);
                itemLabels[i].AutoSize = true;
                itemLabels[i].ForeColor = Color.Black;
                itemLabels[i].BackColor = Color.Transparent;
                itemLabels[i].Parent = pictureBox11;
                itemLabels[i].BringToFront();
            }
        }

        private void InitializeItemArrays()
        {
            // 初始化圖片與價格標籤陣列
            itemPictureBoxes = new PictureBox[] {
                pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
                pictureBox6, pictureBox7, pictureBox8, pictureBox9
            };

            itemLabels = new System.Windows.Forms.Label[] {
                label1, label2,label3, label4, label5, label6, label7, label8, label9
            };

            // 每個商品加入點擊事件（用 index 區分）
            for (int i = 0; i < itemPictureBoxes.Length; i++)
            {
                int index = i;
                itemPictureBoxes[i].Click += (s, e) => PictureBox_Click(index);
            }
        }

        private void InitializeMoney()
        {
            // 起始金額
            int value = random.Next(40, 81) * 100;//4000~8000
            currentMoney = value;
            label11.Text = "$" + currentMoney.ToString();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            // 倒數邏輯
            if (countdown > 0)
            {
                countdown--;
                label10.Text = countdown.ToString();
            }

            // 特定秒數顯示提示圖與文字
            if (countdown == 100 && !isLabelActive)
            {
                label12.Text = "想很久欸!";
                pictureBox10.Image = image1;
                isLabelActive = true;
                labelTimer.Start();
            }

            if (countdown == 70 && !isLabelActive)
            {
                label12.Text = "我想下班...";
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
                CheckGameOver(); // 檢查是否結束遊戲
            }

            UpdatePicture(); // 更新主圖片
        }

        private void CheckGameOver()
        {
            // 判斷遊戲結束條件（錢用完或時間到）
            if (currentMoney == 0 || countdown == 0)
            {
                if (currentMoney == 0)
                    GoToFinishForm(); // 全部買完，勝利
                else
                    GoToLoseForm();   // 時間到，失敗
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
            // 購買商品，扣除金額
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
                // 清空現有圖片與價格
                foreach (var pb in itemPictureBoxes) pb.Image = null;
                foreach (var lbl in itemLabels) lbl.Text = "";

                // 商品清單（圖片與價格）
                var items = new (string imagePath, int price)[]
                {
                    ("Images3/c1.png", 45), ("Images3/d1.png", 65), ("Images3/d2.png", 70),
                    ("Images3/d3.png", 80), ("Images3/d4.png", 90), ("Images3/duck.png", 85),
                    ("Images3/f1.png", 100), ("Images3/f2.png", 120), ("Images3/f3.png", 400),
                    ("Images3/h1.png", 50), ("Images3/r1.png", 110), ("Images3/r2.png", 150)
                };

                // 隨機選 9 項商品並顯示
                var shuffledItems = items.OrderBy(_ => random.Next()).ToArray();
                for (int i = 0; i < itemPictureBoxes.Length; i++)
                {
                    var item = shuffledItems[i];
                    itemPictureBoxes[i].Image = Image.FromFile(item.imagePath);
                    itemPictureBoxes[i].SizeMode = PictureBoxSizeMode.StretchImage;
                    itemPictureBoxes[i].BackColor = Color.Transparent;
                    itemPictureBoxes[i].Parent = pictureBox11;
                    itemLabels[i].Text = "$" + item.price;
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
            if (isLabelActive) return; // 若正在顯示訊息則不更新

            if (isClosingTime)
                pictureBox10.Image = image3; // 顯示打烊圖
            else
                pictureBox10.Image = image2; // 顯示一般圖

            label12.BringToFront(); // 確保訊息在最上層
        }

        private void LabelTimer_Tick(object sender, EventArgs e)
        {
            // 訊息顯示結束後清除
            label12.Text = "";
            isLabelActive = false;
            isClosingTime = false;
            labelTimer.Stop();
            UpdatePicture(); // 回復成一般圖片
        }

        private void label12_Click(object sender, EventArgs e)
        {
            label12.Text = "某些文字"; // 可自訂功能
            UpdatePicture();
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            countdownTimer.Start(); // 點主圖可重新啟動倒數（若暫停）
        }

        private void GoToFinishForm()
        {
            countdownTimer.Stop();
            this.Hide();
            Finish finishForm = new Finish(); // 勝利畫面
            finishForm.Show();
        }

        private void GoToLoseForm()
        {
            countdownTimer.Stop();
            this.Hide();
            Lose loseForm = new Lose(); // 失敗畫面
            loseForm.Show();
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            // 背景圖點擊（目前未使用）
        }
    }
}