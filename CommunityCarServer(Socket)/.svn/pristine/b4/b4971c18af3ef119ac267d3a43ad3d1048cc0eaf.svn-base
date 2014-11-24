using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Web;
using MySql.Data.MySqlClient;
using MySql.Data.Common;

namespace ServerWindowsForms
{

    class DAO
    {
        static public  String name = "null";
        static string connStr="";
        enum Sur : int
        {
            INS = 1,
            DEL = 2,
            SEL = 4,
            UDT = 8,
            NAM = 9,
        }
        public DAO()
        {
            string path = System.Environment.CurrentDirectory + "/../../../conf.ini";
            INIFile inifile = new INIFile(path);
            string section = "Database";
            string username = inifile.IniReadValue(section, "username");
            string password = inifile.IniReadValue(section, "password");
            string server = inifile.IniReadValue(section, "server");
            string database = inifile.IniReadValue(section, "database");
            string port = inifile.IniReadValue(section, "port");
            connStr = @"server=" + server + ";user=" + username + ";database=" + database + ";port=" + port + ";password=" + password + ";";
        }
        public string getName()
        {
            return name;
        }

        public string process(string sql)
        {
            int sur = Convert.ToInt32(sql.Substring(0, 1));
            switch (sur)
            {
                case (Int32)Sur.INS:
                    return (Int_Del(sql.Substring(1)).ToString());
                case (Int32)Sur.DEL:
                    return (Int_Del(sql.Substring(1)).ToString());
                case (Int32)Sur.SEL:
                    return SelectAll(sql.Substring(1));
                case (Int32)Sur.UDT:
                    return CheckUpdate(sql.Substring(1));          
                case (Int32)Sur.NAM:
                    return SelectName(sql.Substring(1));
                default:
                    return "ERROR";
            }
        }





        public int Int_Del(string sql)           
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return 999;

            }
            finally
            {
                conn.Close();
            }
        }

        public string SelectAll(string sql)       
        {
            StringBuilder rtn = new StringBuilder();
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    rtn.Append(rdr[0].ToString());
                    rtn.Append(rdr[7].ToString());
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return "SQLERROR";
            }
            finally
            {
                conn.Close();
            }
            return rtn.ToString();
        }



        public string CheckUpdate(string sql)        
        {
            StringBuilder rtn = new StringBuilder();
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    rtn.Append(rdr[0].ToString());
                    rtn.Append(rdr[7].ToString());
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return "SQLERROR";
            }
            finally
            {
                conn.Close();
            }
            return rtn.ToString();
        }




        public string SelectName(string sql)    
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    name = rdr[1].ToString();
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return "SELNAMEERROR";
            }
            finally
            {
                conn.Close();
            }
            return name;
        }

        public string getpicpath(string pale)
        {
            string path = Environment.CurrentDirectory + "/.." + "/../pic";
            string sql = "SELECT pic FROM record WHERE pale = '"+pale+"'";
            string picname = String.Empty;
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    picname = rdr[0].ToString();
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return "SELNAMEERROR";
            }
            finally
            {
                conn.Close();
            }

            return path + "//" + picname;
        }

    }

}
