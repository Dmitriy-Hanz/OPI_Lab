using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.Sql;
using System.Configuration;
using System.Windows;

namespace ThisGym
{
    class DBreader
    {
        private static SqlConnection con;
        private static SqlDataAdapter dec = new SqlDataAdapter();
        DataTable fullMaster;
        private string sqlConStr;

        public void ConnectionCheck()
        {

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");

            string conStr = "Data Source=.;Database=GymDB;Integrated Security=True";
            try
            {
                using (con = new SqlConnection(conStr))
                { con.Open(); }
                connectionStringsSection.ConnectionStrings["DefaultConnection"].ConnectionString = "Data Source=.;Initial Catalog=GymDB;Integrated Security=True";
            }
            catch
            {
                try
                {
                    conStr = "Data Source=.\\SQLEXPRESS;Database=GymDB;Integrated Security=True";
                    using (con = new SqlConnection(conStr))
                    { con.Open(); }
                    connectionStringsSection.ConnectionStrings["DefaultConnection"].ConnectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=GymDB;Integrated Security=True";
                }
                catch
                {
                    MessageBox.Show("Не удалось установить соединение с базой данных.", "ВНИМАНИЕ!", MessageBoxButton.OK, MessageBoxImage.Error);
                    App.Current.Shutdown();
                }
            }
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");

            try
            {
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                { con.Open(); }//exception class 20 - борохлит подключение к серверу
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                System.IO.File.Delete($"{Environment.CurrentDirectory}\\GymDB_log.ldf");
                using (con = new SqlConnection($"{ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.Split(';')[0]};Database=GymDB;Integrated Security = True"))
                {
                    con.Open();
                    dec.SelectCommand = new SqlCommand($"CREATE DATABASE GymDB ON PRIMARY(FILENAME='{Environment.CurrentDirectory}\\GymDB.mdf') FOR ATTACH", con);
                    dec.SelectCommand.ExecuteNonQuery();
                }
            }

            try
            {
                using (con = new SqlConnection("Data Source =.\\SQLEXPRESS;Database=GymDB;Integrated Security = True"))
                { con.Open(); }
            }
            catch (Exception)
            {
                return;
            }
        }
        public void ReadAll(RealManGym realManObj)
        {
            fullMaster = new DataTable();
            DataTable oberSturmBanMaster;

            // чтение мастеров
            ExecCom("SELECT * FROM GymMaster", fullMaster);
            for (int i = 0; i < fullMaster.Rows.Count; i++)
            {
                realManObj.Masters.Add(new GymMaster((int)fullMaster.Rows[i][0], (string)fullMaster.Rows[i][1].ToString(), (string)fullMaster.Rows[i][2], (string)fullMaster.Rows[i][3], Convert.ToChar(fullMaster.Rows[i][4])));
            }

            // чтение клиентов
            for (int i = 0; i < realManObj.Masters.Count; i++)
            {
                oberSturmBanMaster = new DataTable();
                ExecCom($"SELECT * FROM GymSlave WHERE ID_Мастера_F = {realManObj.Masters[i].gymMasterID}", oberSturmBanMaster);
                for (int j = 0; j < oberSturmBanMaster.Rows.Count; j++)
                {
                    realManObj.Masters[i].Klients.Add(new GymSlave((int)oberSturmBanMaster.Rows[j][0], (string)oberSturmBanMaster.Rows[j][2].ToString(), (string)oberSturmBanMaster.Rows[j][3], (string)oberSturmBanMaster.Rows[j][4], Convert.ToChar(oberSturmBanMaster.Rows[j][5]), (int)oberSturmBanMaster.Rows[j][6]));
                }
            }
        }

        public static void ExecCom(string com)
        {
            using (con = new SqlConnection("Data Source =.\\SQLEXPRESS;Database=GymDB;Integrated Security = True"))
            {
                con.Open();
                dec.SelectCommand = new SqlCommand(com, con);
                dec.SelectCommand.ExecuteNonQuery();
            }
        }
        public static void ExecCom(string com, DataTable filler)
        {
            using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                con.Open();
                dec.SelectCommand = new SqlCommand(com, con);
                dec.Fill(filler);
            }
        }

        //для майн
        public DataTable SearchMaster(string fF,string iI, string oO)
        {
            fullMaster = new DataTable();
            ExecCom($"SELECT * FROM GymMaster WHERE имя like '%{iI}%' and фамилия like '%{fF}%' and отчество like '%{oO}%'", fullMaster);
            return fullMaster;
        }
        public DataTable SearchSlave(string fF, string iI, string oO)
        {
            fullMaster = new DataTable();
            ExecCom($"SELECT ID_Мастера_F,имя,фамилия,отчество,пол,возраст FROM GymSlave WHERE имя like '%{iI}%' and фамилия like '%{fF}%' and отчество like '%{oO}%'", fullMaster);
            return fullMaster;
        }
    }
    class RealManGym
    {
        public List<GymMaster> Masters;
        public RealManGym()
        {
            Masters = new List<GymMaster>();
        }
    }
    class GymMaster
    {
        public int gymMasterID;
        public string name;
        public string surname;
        public string fathername;
        public char gender;
        public List<GymSlave> Klients;

        public GymMaster(int ID,string name, string surname, string fathername, char gender)
        {
            Klients = new List<GymSlave>();
            gymMasterID = ID;
            this.name = name;
            this.surname = surname;
            this.fathername = fathername;
            this.gender = gender;
        }
    }
    class GymSlave
    {
        public int gymSlaveID;
        public string name;
        public string surname;
        public string fathername;
        public char gender;
        public int age;

        public GymSlave(int ID,string name, string surname, string fathername, char gender, int age)
        {
            gymSlaveID = ID;
            this.name = name;
            this.surname = surname;
            this.fathername = fathername;
            this.gender = gender;
            this.age = age;
        }
    }
}
