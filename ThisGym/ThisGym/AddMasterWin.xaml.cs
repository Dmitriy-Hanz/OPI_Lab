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
    public partial class AddMasterWin : Window
    {
        public AddMasterWin()
        {
            InitializeComponent();
        }

        private void CreateMaster_B_Click(object sender, RoutedEventArgs e)
        {
            if (GymMasterName_TB.Text.Length > 60)
            {
                MessageBox.Show("Имя тренера слишком длинное","",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            } 
            else if (GymMasterSurname_TB.Text.Length > 60) 
            {
                MessageBox.Show("Фамилия тренера слишком длинная", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (GymMasterFathername_TB.Text.Length > 60)
            {
                MessageBox.Show("Отчество тренера слишком длинное", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }           
            else if (GymMasterGender_TB.Text != "м" && GymMasterGender_TB.Text != "ж")
            {
                MessageBox.Show("Пол должен обозначатся буквами \"м\" или \"ж\"", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DBreader.ExecCom($"INSERT INTO GymMaster VALUES ('{GymMasterName_TB.Text}','{GymMasterSurname_TB.Text}','{GymMasterFathername_TB.Text}','{GymMasterGender_TB.Text}')");
            LastAddInfo_LB.Content = $"{GymMasterName_TB.Text} {GymMasterSurname_TB.Text} {GymMasterFathername_TB.Text}, {GymMasterGender_TB.Text}, был добавлен в базу данных";
            GymMasterName_TB.Clear();
            GymMasterSurname_TB.Clear();
            GymMasterFathername_TB.Clear();
            GymMasterGender_TB.Clear();
        }

        private void Cancel_B_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
