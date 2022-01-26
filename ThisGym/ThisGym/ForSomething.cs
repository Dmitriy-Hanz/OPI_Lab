using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;

namespace ThisGym
{
    class ForSomething
    {
        public static void KostilFunkMainWin()
        {
            MainWindow.main_W.Dispatcher.BeginInvoke((ThreadStart)delegate ()
            {
                Thread.Sleep(500);
                MainWindow.main_W.UpdateTables_B_Click(null, null);
            });
        }

        public static void KostilFunkReassignWin()
        {
            MainWindow.main_W.Dispatcher.BeginInvoke((ThreadStart)delegate ()
            {
                Thread.Sleep(500);
                MainWindow.reassignSlave_W.GymMasters_DG.Columns[0].Visibility = System.Windows.Visibility.Hidden;
            });
        }
    }
}
