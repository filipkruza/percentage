using System.Runtime.InteropServices;

namespace percentage
{
    class TrayIcon
    {
        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool isSystemDark();

        private const int fontSize = 12;
        private const string font = "Segoe UI";

        private NotifyIcon notifyIcon;

        public TrayIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null, MenuExitClick));
            notifyIcon.Visible = true;

            Update();
            new System.Threading.Timer(Update, null, 0, 2000);
        }

        private static Bitmap GetTextBitmap(String text, Font font, Color fontColor)
        {
            dynamic MeasureStringSize()
            {
                SizeF size;
                using (Image image = new Bitmap(1, 1))
                using (Graphics graphics = Graphics.FromImage(image))
                    size = graphics.MeasureString(text, font);
                return new { Width = (int)size.Width, Height = (int)size.Height };
            }
            Bitmap DrawBitmap(int width, int height)
            {
                var bitmap = new Bitmap(width, height);
                using var graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.FromArgb(0, 0, 0, 0));
                using var brush = new SolidBrush(fontColor);
                graphics.DrawString(text, font, brush, 0, 0);
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                graphics.Save();
                return bitmap;
            }
            var imageSize = MeasureStringSize();
            return DrawBitmap(imageSize.Width, imageSize.Height);
        }

        private void MenuExitClick(object? sender, EventArgs? e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Exit();
        }

        private void Update(object? state = null)
        {
            dynamic GetPowerStrings()
            {
                PowerStatus powerStatus = SystemInformation.PowerStatus;
                String percentage = (powerStatus.BatteryLifePercent * 100).ToString();
                bool isCharging = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;
                var tooltipText = percentage + "%" + (isCharging ? " Charging" : "");
                var bitmapText = percentage;
                return new { bitmap = bitmapText, tooltip = tooltipText };
            }

            var strings = GetPowerStrings();
            var fontColor = (isSystemDark() ? Color.White : Color.Black);
            using var bitmap = new Bitmap(GetTextBitmap(strings.bitmap, new Font(font, fontSize), fontColor));
            using var icon = Icon.FromHandle(bitmap.GetHicon());

            notifyIcon.Icon = icon;
            var toolTipText = strings.tooltip;
            notifyIcon.Text = toolTipText;
        }
    }
}
