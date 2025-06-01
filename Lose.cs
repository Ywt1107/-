using System;                // 引用基本命名空間，提供常用功能（如事件、例外處理等）
using System.Drawing;        // 提供繪圖功能，例如圖片、顏色、字型等
using System.IO;             // 提供檔案與目錄的讀寫功能
using System.Windows.Forms;  // 提供建立 Windows 表單應用程式的功能
//有參考Chatgpt給的框架
namespace 購物達人  // 定義命名空間，用來組織類別，避免命名衝突
{
    public partial class Lose : Form  // 宣告一個名為 Lose 的類別，繼承自 Form 表單類別
    {
        public Lose()  // 建構子：當建立 Lose 表單物件時執行
        {
            InitializeComponent();         // 初始化表單元件（由設計工具產生）
            this.Load += Lose_Load;      // 當表單載入時，執行 Lose_Load 方法
            this.Resize += Lose_Resize;  // 當表單尺寸改變時，執行 Lose_Resize 方法
        }

        private void Lose_Load(object sender, EventArgs e)  // 表單載入事件處理方法
        {
            // 建立圖片路徑：結合應用程式啟動路徑、資料夾名稱、圖片檔案名稱
            string imagePath = Path.Combine(Application.StartupPath, "Images4", "失敗畫面.jpg");

            if (File.Exists(imagePath))  // 如果檔案存在
            {
                pictureBox1.Image = Image.FromFile(imagePath);              // 從檔案載入圖片至 pictureBox1
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;     // 設定圖片模式為拉伸填滿控制項
                pictureBox1.Dock = DockStyle.Fill;                          // 將 pictureBox1 填滿整個表單
            }
            else  // 如果找不到圖片
            {
                MessageBox.Show("找不到圖片：" + imagePath);  // 顯示錯誤訊息
            }

            PositionButton();  // 呼叫自訂方法，設定按鈕位置

            button1.Text = "重新";  // 設定按鈕顯示文字為「重新」
            button1.Font = new Font(button1.Font.FontFamily, 10, FontStyle.Bold);  // 設定按鈕字型為原字型家族、大小 10、粗體
        }

        private void Lose_Resize(object sender, EventArgs e)  // 表單大小變動事件處理方法
        {
            PositionButton();  // 調整按鈕位置，確保仍在右下角
        }

        private void PositionButton()  // 自訂方法：將按鈕定位在表單右下角
        {
            int margin = 10;  // 與邊界的間距
            // 設定 button1 的位置：右下角（扣除按鈕寬度與高度後，再減 margin）
            button1.Location = new Point(this.ClientSize.Width - button1.Width - margin,
                                         this.ClientSize.Height - button1.Height - margin);
        }

        private void button1_Click(object sender, EventArgs e)  // 當按鈕被點擊時執行
        {
            StartForm startForm = new StartForm();  // 建立新的 StartForm 表單（回到開始畫面）
            startForm.Show();                       // 顯示 StartForm 表單
            this.Close();                           // 關閉目前的 Lose 表單
        }
    }
}