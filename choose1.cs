using System;                // 提供基本功能，例如事件、資料型別等
using System.Drawing;        // 提供圖形繪製與處理功能（字型、顏色、圖片等）
using System.IO;             // 提供檔案與目錄存取功能
using System.Windows.Forms;  // 提供建立視窗應用程式的控制項和事件功能
//有參考Chatgpt給的框架
namespace 購物達人  // 命名空間：用來組織程式碼，避免命名衝突
{
    public partial class choose1 : Form  // 定義 choose1 類別，繼承自 Form，表示一個表單
    {
        public choose1()  // 建構子：建立表單物件時執行
        {
            InitializeComponent();        // 初始化控制項（由設計工具產生）
            this.Load += choose1_Load;    // 加入表單載入事件的處理方法
            this.Resize += choose1_Resize;// 加入表單大小改變事件的處理方法
        }

        private void choose1_Load(object sender, EventArgs e)  // 表單載入事件
        {
            // 載入背景圖片
            string imagePath = Path.Combine(Application.StartupPath, "Images4", "主題難易.jpg");
            if (File.Exists(imagePath))  // 如果圖片存在
            {
                pictureBox1.Image = Image.FromFile(imagePath);          // 從檔案載入圖片
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; // 圖片填滿 PictureBox
                pictureBox1.Dock = DockStyle.Fill;                      // PictureBox 填滿整個表單
                pictureBox1.SendToBack();                               // 讓圖片位於最底層
            }
            else
            {
                MessageBox.Show("找不到圖片：" + imagePath); // 若找不到圖片，顯示錯誤訊息
            }

            PositionButtons();  // 呼叫方法設定按鈕位置與尺寸

            // 設定按鈕的顯示文字
            button1.Text = "專櫃";
            button2.Text = "超商";
            button3.Text = "玩具店";
            button4.Text = "返回";

            // 設定按鈕字型為粗體，大小 15（button4 較小為 10）
            button1.Font = new Font(button1.Font.FontFamily, 15, FontStyle.Bold);
            button2.Font = new Font(button2.Font.FontFamily, 15, FontStyle.Bold);
            button3.Font = new Font(button3.Font.FontFamily, 15, FontStyle.Bold);
            button4.Font = new Font(button4.Font.FontFamily, 10, FontStyle.Bold);
        }

        private void choose1_Resize(object sender, EventArgs e)  // 表單調整大小時執行
        {
            PositionButtons();  // 重新調整按鈕位置與大小
        }

        private void PositionButtons()  // 設定按鈕的位置與大小
        {
            // 計算按鈕之間的間距：介於 40 到 80 之間，依表單寬度自動調整
            int spacing = Math.Min(80, Math.Max(40, this.ClientSize.Width / 20));

            // 設定主要按鈕的寬高（最小寬180、高50）
            int buttonWidth = Math.Max(180, this.ClientSize.Width / 6);
            int buttonHeight = Math.Max(50, this.ClientSize.Height / 10);

            // 設定返回按鈕尺寸較小
            int button4Width = Math.Max(80, this.ClientSize.Width / 6);
            int button4Height = Math.Max(30, this.ClientSize.Height / 15);
            button4.Size = new Size(button4Width, button4Height);
            // 設定返回按鈕的位置在左下角（距離左下邊界10px）
            int button4X = 10;
            int button4Y = this.ClientSize.Height - button4Height - 10;
            button4.Location = new Point(button4X, button4Y);

            // 設定三個主要按鈕尺寸
            button1.Size = new Size(buttonWidth, buttonHeight);
            button2.Size = new Size(buttonWidth, buttonHeight);
            button3.Size = new Size(buttonWidth, buttonHeight);

            // 計算三個按鈕總寬度（含間距）
            int totalWidth = buttonWidth * 3 + spacing * 2;
            // 計算按鈕群組的起始 X 位置（置中）
            int startX = (this.ClientSize.Width - totalWidth) / 2;
            // 設定按鈕的 Y 軸位置為畫面垂直中間
            int centerY = this.ClientSize.Height / 2;

            // 設定三個主要按鈕的位置
            button1.Location = new Point(startX, centerY - buttonHeight / 2);
            button2.Location = new Point(startX + buttonWidth + spacing, centerY - buttonHeight / 2);
            button3.Location = new Point(startX + 2 * (buttonWidth + spacing), centerY - buttonHeight / 2);

            // 為三個主要按鈕加上圓角（半徑為 20）
            SetButtonRadius(button1, 20);
            SetButtonRadius(button2, 20);
            SetButtonRadius(button3, 20);
            // 返回按鈕可設定較小圓角（10）
            SetButtonRadius(button4, 10);
        }

        private void SetButtonRadius(Button btn, int radius)  // 設定按鈕圓角的方法
        {
            // 建立繪圖路徑以製作圓角矩形
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);  // 左上角圓角
            path.AddLine(radius, 0, btn.Width - radius, 0);             // 上邊線
            path.AddArc(new Rectangle(btn.Width - radius, 0, radius, radius), -90, 90);  // 右上角
            path.AddLine(btn.Width, radius, btn.Width, btn.Height - radius);            // 右邊線
            path.AddArc(new Rectangle(btn.Width - radius, btn.Height - radius, radius, radius), 0, 90);  // 右下角
            path.AddLine(btn.Width - radius, btn.Height, radius, btn.Height);            // 下邊線
            path.AddArc(new Rectangle(0, btn.Height - radius, radius, radius), 90, 90);  // 左下角
            path.CloseFigure();  // 結束圖形

            btn.Region = new Region(path);  // 將按鈕的外框設定為此圓角圖形
        }

        // 以下為四個按鈕的點擊事件，依據點擊開啟不同的遊戲表單

        private void button1_Click(object sender, EventArgs e)
        {
            _1_1 nextForm = new _1_1();  // 專櫃模式
            nextForm.Show();            // 顯示對應表單
            this.Hide();                // 隱藏目前的 choose1 表單
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _2_1 nextForm = new _2_1();  // 超商模式
            nextForm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _3_1 nextForm = new _3_1();  // 玩具店模式
            nextForm.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            StartForm backForm = new StartForm();  // 回到起始畫面
            backForm.Show();
            this.Close();  // 關閉目前畫面（選擇模式畫面）
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // 圖片被點擊時的事件（目前未實作）
        }
    }
}
