using System.Runtime.InteropServices;

namespace percentage
{
    partial class TrayIcon
    {
        [LibraryImport("UXTheme.dll", EntryPoint = "#138A", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool isSystemDark();

        private readonly NotifyIcon notifyIcon;

        public TrayIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null, MenuExitClick));
            notifyIcon.Visible = true;

            Update();
            new System.Threading.Timer(Update, null, 0, 2000);
        }

        private static Icon GetIcon(String text)
        {
            dynamic stringSize;
            using (var bitmap = new Bitmap(1,1))
            {
                using var graphics = Graphics.FromImage(bitmap);
                var size = graphics.MeasureString(text, new Font("Segoe UI", 16));
                stringSize = new { Width = (int)size.Width, Height = (int)size.Height };
            }
            Icon icon;
            using (var bitmap = new Bitmap(stringSize.Width, stringSize.Height))
            {
                using var graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.FromArgb(0, 0, 0, 0));
                var textColor = (isSystemDark()) ? Color.White : Color.Black;
                using var brush = new SolidBrush(textColor);
                graphics.DrawString(text, new Font("Segoe UI", 16), brush, 0, 0);
                graphics.Save();
                icon = Icon.FromHandle(bitmap.GetHicon());
            }
            return icon;
        }

        private void MenuExitClick(object? sender, EventArgs? e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Exit();
        }

        private void Update(object? state = null)
        {
            var powerStatus = SystemInformation.PowerStatus;
            var percentage = (powerStatus.BatteryLifePercent * 100).ToString();
            var isCharging = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;
            var bitmapText = percentage;
            var tooltipText = percentage + "%" + (isCharging ? " Charging" : "");

            notifyIcon.Icon = GetIcon(bitmapText);
            notifyIcon.Text = tooltipText;
        }
    }
}
