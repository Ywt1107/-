using System;                        // 提供基本的 .NET 功能（如數學運算、日期處理等）
using System.Net;
using System.Drawing;                // 提供圖形功能（如圖片、顏色等）
using System.Linq;                   // 提供 LINQ 查詢語法支援
using System.Reflection.Emit;        // 用於動態建立方法或型別（本程式未使用，可刪除）
using System.Windows.Forms;          // 提供 Windows Form 應用程式 UI 元件支援
//有參考Chatgpt給的框架
namespace 購物達人 // 命名空間：購物達人
{
    public partial class Form3 : Form // Form1 繼承自 Windows Form 表單
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
        private Image image1; // 圖片1：催促圖（33.png）
        private Image image2; // 圖片2：一般狀態圖（11.png）
        private Image image3; // 圖片3：打烊圖（12.png）

        private bool isClosingTime = false; // 是否進入打烊階段
        private bool isLabelActive = false; // 是否正在顯示 label12 文字

        public Form3()
        {
            InitializeComponent(); // 初始化表單元件
            this.Load += Form3_Load; // 註冊載入事件

            // 初始化 label 計時器，顯示 label12 文字 3 秒
            labelTimer = new System.Windows.Forms.Timer();
            labelTimer.Interval = 3000;
            labelTimer.Tick += LabelTimer_Tick;

            try
            {
                // 載入三張主要圖片（透明 PNG）
                image1 = LoadImageFromUrl("https://raw.githubusercontent.com/Ywt1107/-/master/Images/33.png");
                image2 = LoadImageFromUrl("https://raw.githubusercontent.com/Ywt1107/-/master/Images/11.png");
                image3 = LoadImageFromUrl("https://raw.githubusercontent.com/Ywt1107/-/master/Images/12.png");

                // 設定圖片顯示方式與透明背景設定
                pictureBox10.Parent = pictureBox11;             // 讓 pictureBox10 疊在 pictureBox11 上
                pictureBox10.BackColor = Color.Transparent;     // 設定透明背景
                pictureBox10.BringToFront();                    // 確保在最上層
                pictureBox10.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox10.Image = image2;                    // 顯示透明圖片

            }
            catch (Exception ex)
            {
                MessageBox.Show("載入圖片失敗：" + ex.Message);
            }

            UpdatePicture();         // 初始主圖片為 image2（可依邏輯調整）
            InitializeItemArrays();  // 初始化圖片與標籤陣列
            InitializeMoney();       // 隨機產生起始金額（400~900）

            // 初始化倒數計時器
            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000;         // 每秒觸發一次
            countdownTimer.Tick += CountdownTimer_Tick;
            label10.Text = countdown.ToString();      // 顯示初始時間
            countdownTimer.Start();                 // 開始倒數

            LoadItemImages(); // 載入隨機商品與價格
        }

        private Image LoadImageFromUrl(string url)
        {
            using (var wc = new WebClient())
            {
                byte[] data = wc.DownloadData(url);
                using (var ms = new MemoryStream(data))
                {
                    return new Bitmap(ms); // Bitmap 支援透明度
                }
            }
        }



        private void Form3_Load(object sender, EventArgs e)// 畫面載入事件
        {
            LoadImageFromGitHub();

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
            // 將 pictureBox10 設定為 pictureBox11 的子元件
            pictureBox10.Parent = pictureBox11;

            // 設定透明背景
            pictureBox10.BackColor = Color.Transparent;

            // 顯示圖片，範例使用 image2
            pictureBox10.Image = image2;
            pictureBox10.SizeMode = PictureBoxSizeMode.StretchImage;

            // 確保在最上層顯示
            pictureBox10.BringToFront();


            pictureBox10.Left += 60; // 微調位置
            label12.Font = new Font(label12.Font.FontFamily, label12.Font.Size, FontStyle.Bold);// 加粗文字
            label12.Parent = pictureBox10;
            label12.BackColor = Color.Transparent;
            label12.ForeColor = Color.Black;
            label12.Location = new Point(60, 50);
            label12.BringToFront();

            ArrangePictureBoxes(); // 排列商品圖片
            ArrangeLabels();       // 排列價格標籤

            // 初始訊息與圖片
            label12.Text = "歡迎光臨~";
            pictureBox10.Image = image2;// 一般狀態圖片
            isLabelActive = true;
            labelTimer.Start();// 啟動提示計時器
        }
        private async void LoadImageFromGitHub()
        {
            string imageUrl = "https://raw.githubusercontent.com/Ywt1107/-/master/Images4/專櫃遊戲畫面.jpg";

            try
            {
                using (WebClient client = new WebClient())
                {
                    // 從 GitHub 下載圖片成位元組資料
                    byte[] imageBytes = await client.DownloadDataTaskAsync(imageUrl);

                    // 將位元組轉為圖片
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        pictureBox11.Image = Image.FromStream(ms);
                        pictureBox11.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox11.Dock = DockStyle.Fill;
                        pictureBox11.SendToBack(); // 設為背景
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("圖片載入失敗：" + ex.Message);
            }
        }
        private void ArrangePictureBoxes()
        {
            // 商品圖片排列（3列3行）
            int startX = 40, startY = 38;// 起始座標
            int boxWidth = 100, boxHeight = 100;// 每個商品框大小
            int spacingX = 70, spacingY = 30;// 間距

            for (int i = 0; i < itemPictureBoxes.Length; i++)
            {
                int row = i / 3, col = i % 3; // 換算行列
                int x = startX + col * (boxWidth + spacingX);
                int y = startY + row * (boxHeight + spacingY);

                itemPictureBoxes[i].Location = new Point(x, y);
                itemPictureBoxes[i].Size = new Size(boxWidth, boxHeight);
                itemPictureBoxes[i].BringToFront();// 顯示在最上層
            }
        }

        private void ArrangeLabels()
        {
            // 價格標籤位置與屬性設定
            int offsetX = 30, offsetY = 105;// 價格標籤相對於圖片的位置

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
            // 起始金額（400~900 間的整十數）
            int value = random.Next(50, 101) * 100;
            currentMoney = value;
            label11.Text = "$" + currentMoney.ToString();// 顯示起始金額
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
                    GoToFinishForm(); // 全部買完，勝利// 金額歸零，勝利
                else
                    GoToLoseForm();   // 時間到，失敗// 時間歸零，失敗
            }
        }

        private void PictureBox_Click(int index)// 當商品圖片被點擊時執行，參數 index 表示點擊的是哪一個商品
        {
            var pb = itemPictureBoxes[index];// 取得被點擊的 PictureBox 物件
            if (pb.Tag is int price)// 判斷該 PictureBox 的 Tag 屬性是否為整數（即商品價格）
            {
                BuyItem(price);// 購買此商品// 呼叫購買商品的方法，並傳入價格參數
            }
        }

        private void BuyItem(int price)// 購買商品的方法，參數 price 是商品價格
        {
            // 購買商品，扣除金額// 判斷玩家是否有足夠金錢購買該商品
            if (currentMoney >= price)
            {
                currentMoney -= price;// 扣除商品價格，更新玩家剩餘金額
                label11.Text = "$" + currentMoney.ToString();// 更新畫面上的金錢顯示
                CheckGameOver();// 檢查是否結束
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
                string baseUrl = "https://raw.githubusercontent.com/Ywt1107/-/master/Images/";

                // 商品清單（圖片與價格）
                var items = new (string fileName, int price)[]
                {
                    ("LV.png", 50), ("OO.png", 60), ("cheap.png", 70),
                    ("n1.png", 80), ("n2.png", 90), ("n3.png", 100),
                    ("e1.png", 135), ("e2.png", 165), ("e3.png", 195),
                    ("e4.png", 200), ("m1.png", 150), ("m2.png", 120),
                    ("m3.png", 160), ("m4.png", 180), ("m5.png", 350),
                    ("m6.png", 500), ("m7.png", 480), ("m8.png", 270),
                    ("bag.png", 340)
                };

                // 隨機選 9 項商品並顯示
                var shuffledItems = items.OrderBy(_ => random.Next()).ToArray();// 將商品清單隨機打亂順序後存入新陣列
                for (int i = 0; i < itemPictureBoxes.Length; i++)// 將前 9 項商品依序顯示在畫面上
                {
                    var item = shuffledItems[i];// 取得當前商品資訊（圖片路徑與價格）
                    string imageUrl = baseUrl + item.fileName;
                    itemPictureBoxes[i].Image = LoadImageFromUrl(imageUrl);// 載入圖片並設定給對應的 PictureBox
                    itemPictureBoxes[i].SizeMode = PictureBoxSizeMode.StretchImage;// 設定圖片顯示模式為填滿 PictureBox
                    itemPictureBoxes[i].BackColor = Color.Transparent;// 設定圖片的背景為透明
                    itemPictureBoxes[i].Parent = pictureBox11;// 設定圖片的容器為主背景圖片 PictureBox（可達成透明覆蓋）
                    itemLabels[i].Text = "$" + item.price;// 將商品價格文字顯示在對應的 Label 上
                    itemPictureBoxes[i].Tag = item.price;// 將價格儲存在 PictureBox 的 Tag 屬性中，方便後續取用
                }
            }
            catch (Exception ex)// 若發生錯誤（例如找不到圖片），則顯示錯誤訊息
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