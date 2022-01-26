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
using System.Windows.Shapes;


namespace ThisGym
{
    public partial class AddSlaveWin : Window
    {
        private int masterID;
        public AddSlaveWin(object masterID)
        {
            InitializeComponent();
            this.masterID = (int)masterID;
        }

        private void CreateSlave_B_Click(object sender, RoutedEventArgs e)
        {
            if (GymSlaveName_TB.Text.Length > 60)
            {
                MessageBox.Show("Имя тренера слишком длинное","",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            } 
            else if (GymSlaveSurname_TB.Text.Length > 60) 
            {
                MessageBox.Show("Фамилия тренера слишком длинная", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (GymSlaveFathername_TB.Text.Length > 60)
            {
                MessageBox.Show("Отчество тренера слишком длинное", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }           
            else if (GymSlaveGender_TB.Text != "м" && GymSlaveGender_TB.Text != "ж")
            {
                MessageBox.Show("Пол должен обозначатся буквами \"м\" или \"ж\"", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }            
            else if (int.TryParse(GymSlaveAge_TB.Text,out int r) != true)
            {
                MessageBox.Show("Введенное значение в поле возраста не соответствует требуемому формату", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DBreader.ExecCom($"INSERT INTO GymSlave VALUES ('{masterID}','{GymSlaveName_TB.Text}','{GymSlaveSurname_TB.Text}','{GymSlaveFathername_TB.Text}','{GymSlaveGender_TB.Text}','{GymSlaveAge_TB.Text}')");
            LastAddInfo_LB.Content = $"{GymSlaveName_TB.Text} {GymSlaveSurname_TB.Text} {GymSlaveFathername_TB.Text}, {GymSlaveGender_TB.Text}, {GymSlaveAge_TB.Text}, был добавлен в базу данных";
            GymSlaveName_TB.Clear();
            GymSlaveSurname_TB.Clear();
            GymSlaveFathername_TB.Clear();
            GymSlaveGender_TB.Clear();
            GymSlaveAge_TB.Clear();
        }

        private void Cancel_B_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
