using System.Threading;
using System.Windows.Forms;

using Yasuno.Properties;

namespace Yasuno
{
    internal class Program
    {
        private const string MutexName = "YasuNoMutex";

        private static void Main(string[] args)
        {
            using (var mutex = new Mutex(true, Program.MutexName, out var created))
            {
                if (!created)
                {
                    return;
                }

                var notifyThread = new Thread(
                    () =>
                    {
                        var notifyIcon = new NotifyIcon
                                         {
                                             Icon = Resources.NOYasuo_1,
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
}
