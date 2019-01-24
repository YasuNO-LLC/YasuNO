using System.Threading;
using System.Windows.Forms;

using Yasuno.Properties;

namespace Yasuno
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var notifyThread = new Thread(
                () =>
                {
                    var notifyIcon = new NotifyIcon
                                     {
                                         Icon = Resources.Icon1,
                                         Text = "YasuNO",
                                         BalloonTipText = "YasuNO is now protecting you from yourself",
                                         Visible = true
                                     };

                    notifyIcon.ShowBalloonTip(2);
                    Application.Run();
                }
            );

            notifyThread.SetApartmentState(ApartmentState.STA);
            notifyThread.Start();

            using (new YasuNo())
            {
                notifyThread.Join();
            }
        }
    }
}
