using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;

namespace ThisGym
{
    public partial class ReassignSlaveWin : Window
    {
        private int slaveID;
        public ReassignSlaveWin(object slaveID)
        {
            InitializeComponent();
            DataTable fullMaster = new DataTable();
            DBreader.ExecCom($"SELECT * FROM GymMaster", fullMaster);
            GymMasters_DG.ItemsSource = fullMaster.DefaultView;
            this.slaveID = (int)slaveID;
            Thread kostil = new Thread(ForSomething.KostilFunkReassignWin);
            kostil.Start();
        }

        private void Cancel_B_Click(object sender, RoutedEventArgs e) => this.Close();

        private void ReassignSlave_B_Click(object sender, RoutedEventArgs e)
        {
            if (GymMasters_DG.SelectedItem as DataRowView != null)
            {
                DBreader.ExecCom($"UPDATE GymSlave SET ID_Мастера_F = {(GymMasters_DG.SelectedItem as DataRowView).Row.ItemArray[0]} WHERE ID_Клиента_P = {slaveID}");
                Close();
            }
            else
            {
                MessageBox.Show("Вы должны выбрать тренера, к которому приписываете клиента", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
