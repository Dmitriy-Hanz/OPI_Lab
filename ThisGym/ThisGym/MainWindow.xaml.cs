using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Data.Sql;
using System.Configuration;
using System.Threading;

namespace ThisGym
{
    public partial class MainWindow : Window
    {
        RealManGym rmg = new RealManGym();
        DBreader dbr;
        DataTable fullMaster = new DataTable();

        public static MainWindow main_W;
        public static ReassignSlaveWin reassignSlave_W;

        public MainWindow()
        {
            InitializeComponent();
            main_W = this;
            dbr = new DBreader();
            dbr.ConnectionCheck();
            dbr.ReadAll(rmg);
            Thread kostil = new Thread(ForSomething.KostilFunkMainWin);
            kostil.Start();
        }

        private void SearchMaster_B_Click(object sender, RoutedEventArgs e)
        {
            GymMasters_DG.ItemsSource = dbr.SearchMaster(GymMasterName_TB.Text, GymMasterSurname_TB.Text, GymMasterFathername_TB.Text).DefaultView;
            GymMasters_DG.Columns[0].Visibility = Visibility.Hidden;
        }
        private void SearchSlave_B_Click(object sender, RoutedEventArgs e)
        {
            GymSlaves_DG.ItemsSource = dbr.SearchSlave(GymSlaveName_TB.Text, GymSlaveSurname_TB.Text, GymSlaveFathername_TB.Text).DefaultView;
            GymSlaves_DG.Columns[0].Visibility = Visibility.Hidden;
        }

        private void GymMasters_DG_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (GymMasters_DG.SelectedItem as DataRowView != null)
            {
                DeleteMaster_B.IsEnabled = true;
                AddSlave_B.IsEnabled = true;
                fullMaster = new DataTable();
                DBreader.ExecCom($"SELECT фамилия,имя,отчество,пол,возраст FROM GymSlave WHERE ID_Мастера_F = {(GymMasters_DG.SelectedItem as DataRowView).Row.ItemArray[0]}", fullMaster);
                SlavesToMaster_DG.ItemsSource = fullMaster.DefaultView;
                return;
            }
            SlavesToMaster_DG.ItemsSource = null;
            DeleteMaster_B.IsEnabled = false;
            AddSlave_B.IsEnabled = false;
        }
        private void MasterToSlaves_DG_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (MasterToSlaves_DG.SelectedItem as DataRowView != null)
            {
                DeleteMaster_B.IsEnabled = true;
                return;
            }
            DeleteMaster_B.IsEnabled = false;
        }
        private void GymSlaves_DG_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (GymSlaves_DG.SelectedItem as DataRowView != null)
            {
                DeleteSlave_B.IsEnabled = true;
                ReassignSlave_B.IsEnabled = true;
                if ((GymSlaves_DG.SelectedItem as DataRowView).Row.ItemArray[1] != DBNull.Value)
                {
                    fullMaster = new DataTable();
                    DBreader.ExecCom($"SELECT имя,фамилия,отчество,пол FROM GymMaster WHERE ID_Мастера_P = {(GymSlaves_DG.SelectedItem as DataRowView).Row.ItemArray[1]}", fullMaster);
                    MasterToSlaves_DG.ItemsSource = fullMaster.DefaultView;
                    return;
                }
                MasterToSlaves_DG.ItemsSource = null;
                return;
            }
            MasterToSlaves_DG.ItemsSource = null;
            DeleteSlave_B.IsEnabled = false;
            ReassignSlave_B.IsEnabled = false;
        }
        private void SlavesToMaster_DG_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (SlavesToMaster_DG.SelectedItem as DataRowView != null)
            {
                DeleteSlave_B.IsEnabled = true;
                AddSlave_B.IsEnabled = true;
                return;
            }
            DeleteSlave_B.IsEnabled = false;
        }



        public void UpdateTables_B_Click(object sender, RoutedEventArgs e)
        {
            SlavesToMaster_DG.ItemsSource = MasterToSlaves_DG.ItemsSource = null;
            fullMaster = new DataTable();
            DBreader.ExecCom($"SELECT * FROM GymMaster", fullMaster);
            GymMasters_DG.ItemsSource = fullMaster.DefaultView;
            fullMaster = new DataTable();
            DBreader.ExecCom($"SELECT * FROM GymSlave", fullMaster);
            GymSlaves_DG.ItemsSource = fullMaster.DefaultView;
            GymSlaves_DG.Columns[0].Visibility = GymMasters_DG.Columns[0].Visibility = Visibility.Hidden;
            GymSlaves_DG.Columns[1].Visibility = GymMasters_DG.Columns[0].Visibility = Visibility.Hidden;
        }

        private void AddSlave_B_Click(object sender, RoutedEventArgs e)
        {
            AddSlaveWin addSlave_W = new AddSlaveWin((GymMasters_DG.SelectedItem as DataRowView).Row.ItemArray[0]);
            addSlave_W.ShowDialog();
            UpdateTables_B_Click(null, null);
        }

        private void AddMaster_B_Click(object sender, RoutedEventArgs e)
        {
            AddMasterWin addMaster_W = new AddMasterWin();
            addMaster_W.ShowDialog();
            UpdateTables_B_Click(null, null);
        }

        private void DeleteSlave_B_Click(object sender, RoutedEventArgs e)
        {
            if (GymSlaves_DG.SelectedItem as DataRowView == null)
            {
                MessageBox.Show("Не выбран клиент для удаления", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (MessageBox.Show("Вы уверены?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) { return; }

            DBreader.ExecCom($"DELETE FROM GymSlave WHERE ID_Клиента_P = {(GymSlaves_DG.SelectedItem as DataRowView).Row.ItemArray[0]}");
            UpdateTables_B_Click(null, null);
        }

        private void DeleteMaster_B_Click(object sender, RoutedEventArgs e)
        {
            if (GymMasters_DG.SelectedItem as DataRowView == null)
            {
                MessageBox.Show("Не выбран тренер для удаления", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (MessageBox.Show("В случае удаления данного тренера все его клиенты будут перезаписаны на другого тренера. Вы уверены?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) { return; }

            fullMaster = new DataTable();

            DBreader.ExecCom($"UPDATE GymSlave SET ID_Мастера_F = NULL WHERE ID_Мастера_F = {(GymMasters_DG.SelectedItem as DataRowView).Row.ItemArray[0]}");
            DBreader.ExecCom($"DELETE FROM GymMaster WHERE ID_Мастера_P = {(GymMasters_DG.SelectedItem as DataRowView).Row.ItemArray[0]}");

            DBreader.ExecCom("SELECT * FROM GymMaster", fullMaster);
            if (fullMaster.Rows.Count != 0)
            {
                DBreader.ExecCom($"UPDATE GymSlave SET ID_Мастера_F = (SELECT TOP(1) ID_Мастера_P FROM GymMaster) WHERE ID_Мастера_F IS NULL");
            }
            
            SlavesToMaster_DG.ItemsSource = fullMaster.DefaultView;
            UpdateTables_B_Click(null, null);
        }


        private void ReassignSlave_B_Click(object sender, RoutedEventArgs e)
        {
            reassignSlave_W = new ReassignSlaveWin((GymSlaves_DG.SelectedItem as DataRowView).Row.ItemArray[0]);
            reassignSlave_W.ShowDialog();
            UpdateTables_B_Click(null, null);
        }
    }
}
