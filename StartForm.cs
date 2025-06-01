using System;                      // 提供基本的 .NET 功能（例如事件、日期時間、基礎型別等）
using System.Net;
using System.Collections.Generic;  // 提供泛型集合（如 List<T> 等）
using System.ComponentModel;       // 提供元件設計階段支援（少用）
using System.Data;                 // 提供資料庫處理功能（這裡未使用）
using System.Drawing;              // 提供圖形繪製與顯示功能（顏色、圖片、字型等）
using System.IO;                   // 提供檔案與目錄處理功能
using System.Linq;                 // 提供 LINQ 語法支援
using System.Text;                 // 提供字串編碼功能（這裡未使用）
using System.Threading.Tasks;      // 提供非同步程式設計功能（這裡未使用）
using System.Windows.Forms;        // 提供表單與控制項功能
//有參考Chatgpt給的框架
namespace 購物達人  // 命名空間：用來組織程式碼，避免類別名稱衝突
{
    public partial class StartForm : Form  // StartForm 類別，繼承自 Form，表示一個視窗表單
    {
        public StartForm()  // 建構子：初始化 StartForm 表單
        {
            InitializeComponent();        // 初始化表單控制項（由 Visual Studio 自動產生）
            this.Load += StartForm_Load;  // 加入表單載入事件處理方法
            this.Resize += StartForm_Resize; // 加入表單縮放事件處理方法
        }

        private void button1_Click(object sender, EventArgs e)  // 按下按鈕1 的事件處理方法（未使用）
        {
            choose1 chooseForm = new choose1();  // 建立新的 choose1 表單
            chooseForm.Show();                   // 顯示 choose1 表單
            this.Hide();                         // 隱藏當前的 StartForm 表單
        }

        private void StartForm_Load(object sender, EventArgs e)  // 表單載入時觸發
        {
            LoadImageFromGitHub();

            // 將按鈕加入 pictureBox1 的控制項，讓它顯示在圖片上層
            pictureBox1.Controls.Add(start);

            // 設定按鈕背景為透明白（使用 ARGB 設定透明度）
            start.BackColor = Color.FromArgb(200, Color.White); // 白色 + 半透明（透明度 200）

            // 設定按鈕樣式為扁平樣式
            start.FlatStyle = FlatStyle.Flat;

            // 設定按鈕字體大小與樣式（粗體）
            start.Font = new Font(start.Font.FontFamily, 10, FontStyle.Bold);

            // 呼叫方法設定按鈕的位置
            PositionButton();
        }
        private async void LoadImageFromGitHub()
        {
            string imageUrl = "https://raw.githubusercontent.com/Ywt1107/-/master/Images4/開始畫面.jpg";

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
        private void StartForm_Resize(object sender, EventArgs e)  // 當表單尺寸變動時觸發
        {
            PositionButton();  // 重新設定按鈕位置，確保仍置中對齊
        }

        // 設定按鈕位置的方法
        private void PositionButton()
        {
            // 計算讓按鈕置中於 pictureBox1 的 X 座標
            int buttonX = (pictureBox1.Width - start.Width) / 2;

            // 計算按鈕 Y 座標，距離底部 20px
            int buttonY = pictureBox1.Height - start.Height - 20;

            // 設定按鈕的新位置
            start.Location = new Point(buttonX, buttonY);
        }

        private void pictureBox1_Click(object sender, EventArgs e)  // 點擊圖片的事件處理方法（尚未使用）
        {
            // 此處目前未做任何處理，可用於未來圖片點擊功能
        }
    }
}
