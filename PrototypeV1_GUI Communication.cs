using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;
using Test;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Net.Sockets;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Utils;
using System.Diagnostics;


namespace ClientAppV1
{
   
    public partial class Device1 : Form
    {
        
        private bool StopMonitoring = false;
        private Utility Test = new Utility();
        private AlramHandler AlarmHanlder;


        public Device1()
        {
            InitializeComponent();
        }

        private void Device1_Load(object sender, EventArgs e)
        {
            //IPAddr.Text = "192.168.0.7";
        }

        public void but_Connect_Click(object sender, EventArgs e)
        {
            //Connect with the Modbus device
          //Add your code here//
        }
        //Read Device Info
      
        //Read registers and write data to data base
     
       // private static ICollection SetPaths(ScriptEngine engine)
       // {

       // }

        private void but_Dev1Start_Click(object sender, EventArgs e)
        {

            //Only Admin can start counting
            //TBD: Strict Type checking
           
            if (Utility.Accesslevel == "Admin" || Utility.Admin == true)
            {
                Int32 timeInMs = (Convert.ToInt32(Interval.Text) * 3000);
                tmr_Device1.Interval = timeInMs;
                tmr_Device1.Enabled = true;
                DateTime endTime = DateTime.Now;
                CommWithServer();

             
          }//End if
          else { MessageBox.Show("You dont have permission to perform this operation-2"); } //TBD: Better way to present?? }*/

        }//End Start Command
        private void CommWithServer()
        {
            string key = "hello";
            string fileName = "C://Users//shwethag//AppData//Local//Programs//Python//Python36//Test.py";
            string EncryptedData = "hello";
            float Temp, Humidity;
            string[] TempData;
            try
            {


                TcpClient tcpClnt = new TcpClient();
                tcpClnt.Connect("192.168.137.225", 502);
                String requestKey = "Read";
                Stream stm1 = tcpClnt.GetStream();
                ASCIIEncoding asen1 = new ASCIIEncoding();
                byte[] ba1 = asen1.GetBytes(requestKey);
                stm1.Write(ba1, 0, ba1.Length);
                byte[] RxData = new byte[1024];
                stm1.Read(RxData, 0, 1024);
                string strData = System.Text.Encoding.UTF8.GetString(RxData);
               // MessageBox.Show(strData);
                string[] strTmpData = strData.Split(',');
                key = strTmpData[1];
              //  MessageBox.Show(key);
                EncryptedData = strTmpData[0];
                string sep = ",";
                string SendDataToPython = EncryptedData + sep + key;
               // MessageBox.Show(SendDataToPython);

                string cmd = @"C:\Users\shwethag\AppData\Local\Programs\Python\Python36\python.exe";
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = cmd;
                //start.Arguments = fileName;
                start.Arguments = string.Format("{0} {1} {2}", fileName, SendDataToPython, EncryptedData);

                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string DecryptedData = reader.ReadToEnd();
                       // MessageBox.Show(DecryptedData);
                        // string DecryptedData = System.Text.Encoding.UTF8.GetString(result);
                        TempData = DecryptedData.Split(',');
                        lbl_Temp.Text = TempData[1];
                        lbl_Humid.Text = TempData[0];
                    }
                }
                tcpClnt.Close();
                //Write Data to SQL
                Temp = float.Parse(TempData[1]);
                Humidity = float.Parse(TempData[0]);
                writeToSQL(Temp, Humidity);
                //Check Alarms: Threshold levels are hardcoded
             
                if ((Temp >=float.Parse(txtTempThd.Text)))
                {
                    //Generate alarm
                    string devName = "Sensor:1";
                    string AlarmInfo = "Temperature is" + Temp.ToString() + "Out of Range";
                    AlarmHanlder = new AlramHandler();
                    AlarmHanlder.AlarmHandle(AlarmInfo, devName);

                }
                if ((Humidity >= float.Parse(txtHumThd.Text)))
                {
                    //Generate alarm
                    string devName = "Sensor:1";
                    string AlarmInfo = "Humidity is" + Humidity.ToString() + "Out of Range";
                    AlarmHanlder = new AlramHandler();
                    AlarmHanlder.AlarmHandle(AlarmInfo, devName);

                }

            }
            

            catch (SystemException error)
            {
                MessageBox.Show("Device1-Read Error" + error.Message);

            }
            



    } //End of Comm Function

        private void writeToSQL(float Temp, float Humidity)
        {
           
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server= EQUIP-20L-GOWD\\FTVIEWX64TAGDB;Database=TestDb;Trusted_Connection=true"; //erver= ENVSERVER\\HUA_OPCSERVER;Database=HUAOPCDB;Trusted_Connection=true";
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "INSERT into ProjectDemo(DateTime,Temp,Humidity)VALUES(@DateTime,@Temp,@Humidity)";
                    //command.Parameters.AddWithValue("@LogDate", DateTime.Now.ToString("yyyy/MM/dd"));
                    command.Parameters.AddWithValue("@DateTime", DateTime.Now);
                    command.Parameters.AddWithValue("@Temp", Temp);
                    command.Parameters.AddWithValue("@Humidity", Humidity);
                    try
                    {
                        conn.Open();
                        int result = command.ExecuteNonQuery();

                    }
                    catch (SqlException e)
                    {
                        MessageBox.Show("Eror wring to Sql" + e.Message);
                    }


                }


            }
        } //End of WriteSql Function

        /
        private void tmr_Device1_Tick(object sender, EventArgs e)
        {
            DateTime stTime = DateTime.Now;

            CommWithServer();

            DateTime endTime = DateTime.Now;
            //    MessageBox.Show(Convert.ToString(endTime-stTime) + Convert.ToString(endTime) + Convert.ToString(stTime));

        }

        private void but_Dev1Stop_Click(object sender, EventArgs e)
        {
            if (Utility.Accesslevel == "Admin" || Utility.Admin == true)
            {
                tmr_Device1.Enabled = false;
            }
            else { MessageBox.Show("You dont have permission to perform this operation-3"); }

        }
        //Database Function
        //TOFO: Add Database class 
        private void but_Trend_Click(object sender, EventArgs e)
        {
            Devce1Plot3 db = new Devce1Plot3();
            db.Show();
            // Dev1Plot db1 = new Dev1Plot();
            //   db1.Show();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Device1Rpt report = new Device1Rpt();
            report.Show();
        }

        //Brings up statics screen for device1
        
        public void Disconnect(object sender, EventArgs e)
        {
            if (Utility.Accesslevel == "Admin" || Utility.Admin == true)
            {
                

                //Stop the timer
                tmr_Device1.Enabled = false;
                //stop the counter
                but_Dev1Stop_Click(null, null);
                Utility.Admin = false;
            }
            else { MessageBox.Show("You dont have permission to perform this operation"); }

        }

     
        private void Device1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

      
    }
}

