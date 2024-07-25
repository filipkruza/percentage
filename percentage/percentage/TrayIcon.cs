using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace percentage
{
    class TrayIcon
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool DestroyIcon(IntPtr handle);
        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool isSystemDark();

        private const int fontSize = 18;
        private const string font = "Segoe UI";

        private NotifyIcon notifyIcon;

        public TrayIcon()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();

            notifyIcon = new NotifyIcon();

            contextMenu.MenuItems.AddRange(new MenuItem[] { menuItem });

            menuItem.Click += new System.EventHandler(MenuExitClick);
            menuItem.Index = 0;
            menuItem.Text = "Exit";

            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Visible = true;

            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(Update);
            timer.Start();
        }

        private Bitmap GetTextBitmap(String text, Font font, Color fontColor)
        {
            (int,int) MeasureStringSize()
            {
                SizeF size;
                using (Image image = new Bitmap(1, 1))
                    using (Graphics graphics = Graphics.FromImage(image))
                        size = graphics.MeasureString(text, font);
                return ((int)size.Width, (int)size.Height);
            }
            Bitmap DrawBitmap(int width, int height)
            {
                Bitmap bitmap = new Bitmap(width, height);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.FromArgb(0, 0, 0, 0));
                    using (Brush brush = new SolidBrush(fontColor))
                    {
                        graphics.DrawString(text, font, brush, 0, 0);
                        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        graphics.Save();
                    }
                }
                return bitmap;
            }
            var imageSize = MeasureStringSize();
            return DrawBitmap(imageSize.Item1, imageSize.Item2);
        }

        private void MenuExitClick(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Exit();
        }

        private void Update(object sender, EventArgs e)
        {
            (String, String) GetPowerStrings()
            {
                PowerStatus powerStatus = SystemInformation.PowerStatus;
                String percentage = (powerStatus.BatteryLifePercent * 100).ToString();
                bool isCharging = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;
                String tooltipText = percentage + "%" + (isCharging ? " Charging" : "");
                String bitmapText = percentage;
                if (percentage == "100")
                    bitmapText = "F";
                return (bitmapText, tooltipText);
            }

            Bitmap CreateBitmap(String BitmapText, Color BitmapColor)
            {
                return new Bitmap(GetTextBitmap(BitmapText, new Font(font, fontSize), BitmapColor));
            }

            (String, String) texts = GetPowerStrings();
            Color fontColor = (isSystemDark() ? Color.White : Color.Black);
            using (Bitmap bitmap = CreateBitmap(texts.Item1,fontColor))
            {
                IntPtr intPtr = bitmap.GetHicon();
                try
                {
                    using (Icon icon = Icon.FromHandle(intPtr))
                    {
                        notifyIcon.Icon = icon;
                        String toolTipText = texts.Item2;
                        notifyIcon.Text = toolTipText;
                    }
                }
                finally
                {
                    DestroyIcon(intPtr);
                }
            }
        }
    }
}
