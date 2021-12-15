using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.IO;
using System.Net.Sockets;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Data;


public partial class _Default : Page
   
{
   // private static Timer Timer1 = new Timer();
    string IPAddr;
    private Utility test = new Utility();
    private static bool Permit= false;
    private static int MesId = 0;
    private Random rand = new Random();
     protected void Page_Load(object sender, EventArgs e)
    {

    }

    //Start Program
    protected void Button1_Click(object sender, EventArgs e)
    {
        
        Int32 timeInMs = (Convert.ToInt32(1) * 3000);
        Timer1.Interval = timeInMs;
        Timer1.Enabled = true;
        DateTime endTime = DateTime.Now;
       // CommWithServer();
        CommWithServer();
        /* if(null== Utility.UserName || "" == Utility.UserName)
         {
             MesId = 1;
          }
         else if(null == Utility.Accesslevel || "" == Utility.Accesslevel)
         {
             MesId = 2;
         }
         else if (Utility.Accesslevel!="Admin" && Utility.Accesslevel != "Operator")
         {
             MesId = 3;
         }
         else if (Utility.Accesslevel == "Admin" || Utility.Accesslevel == "Operator")
         {
             Int32 timeInMs = (Convert.ToInt32(1) * 3000);
             Timer1.Interval = timeInMs;
             Timer1.Enabled = true;
             DateTime endTime = DateTime.Now;
             CommWithServer();
         }*/

    }
    //Communication Handler
    private void CommWithServer()
    {
        string key = "hello";
        string fileName = "C://Users//shwethag//AppData//Local//Programs//Python//Python36//Test.py";
        string EncryptedData = "hello";
        float Temp, Humidity;
        string[] TempData;
       /* try
        {


            TcpClient tcpClnt = new TcpClient();
            IPAddr = Txt_IPAddr.Text;
            tcpClnt.Connect(IPAddr, 502);
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
                    Txt_Temp.Text = TempData[1];
                    Txt_Humid.Text = TempData[0];
                }
            }
            tcpClnt.Close();
            //Write Data to SQL
            Temp = float.Parse(TempData[1]);
            Humidity = float.Parse(TempData[0]);
            writeToSQL(Temp, Humidity);
            //Check Alarms: Threshold levels are hardcoded

            /* if ((Temp >= float.Parse(txtTempThd.Text)))
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

             }*/

      //  }
       // catch (SystemException error)
       // {
            ; //  MessageBox.Show("Device1-Read Error" + error.Message);

        //}
        

        writeToSQL("AK Sensor 1", 1);
        writeToSQL("AK Sensor 2", 2);
       // writeToSQL("PA-Sensor", 3);

    }//End of Communication Handler

   
    private void writeToSQL(string dspl, int SnsId)
    {
        SqlDataReader rdr;
        double temp=0.0;
        string Rdtime, Sensor;
        using (SqlConnection conn = new SqlConnection())
        {
            //  conn.ConnectionString = "Server= EQUIP-20L-GOWD\\FTVIEWX64TAGDB;Database=TestDb;Trusted_Connection=true"; //erver= ENVSERVER\\HUA_OPCSERVER;Database=HUAOPCDB;Trusted_Connection=true";
            conn.ConnectionString = "Server=tcp:securecroptempserver.database.windows.net,1433; Initial Catalog = securecroptempsqldb; Persist Security Info = False; User ID = securecroptempserveradmin; Password = S3cur3Cr0padmin12031!; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30";
            using (SqlCommand command = new SqlCommand())
            {
            //Txt_Debug.Text = "one";
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                // command.CommandText = "SELECT UserName, Password, AccessRights FROM Users WHERE((UserName=@User) AND (Password= @Pwd))";
                 command.CommandText = "SELECT Top (1) time, dspl,temp  FROM securecroptemptbl WHERE((dspl=@dspl)) ORDER BY time DESC  ";
              //  command.CommandText = "SELECT time, dspl,temp  FROM securecroptemptbl WHERE((dspl=@dspl))";
                command.Parameters.AddWithValue("@dspl", dspl);
               // command.Parameters.AddWithValue("@displ", displ);

                try
                {
                    conn.Open();
                    rdr = command.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        //Txt_Debug.Text = "two";
                        // MessageBox.Show("Login Success");
                        while (rdr.Read())
                        {
                           // Txt_Debug.Text = "three";
                            Rdtime = rdr.GetString(0);
                            temp = rdr.GetDouble(2);
                            temp = temp + rand.NextDouble();
                            Sensor = rdr.GetString(1);
                            if(SnsId==1)
                            {
                                Txt_Temp1.Text = temp.ToString();
                                Text_Sns1.Text = Sensor;
                            }
                            if(SnsId == 2)
                            {
                                txt_Temp2.Text = temp.ToString();
                                Txt_Sns2.Text= Sensor;

                            }
                            if (SnsId == 3 )
                            {
                               txt_Temp3.Text= temp.ToString();
                               // Tex.Text = Sensor;

                            }
                            //xt_Time.Text = Rdtime;
                            //string Time = Rdtime;


                        }


                    }
                }
                catch (Exception e)
                {
                    ;
                }
            }
            conn.Close();


        }
       

    } //End of WriteSql Function



    protected void Timer1_Tick(object sender, EventArgs e)
    {
        DateTime stTime = DateTime.Now;
        CommWithServer();
        DateTime endTime = DateTime.Now;
    }

    //Stop Fundtions
    protected void Button2_Click(object sender, EventArgs e)
    {
        if (null == Utility.UserName || "" == Utility.UserName)
        {
            MesId = 1;
        }
        else if (null == Utility.Accesslevel || "" == Utility.Accesslevel)
        {
            MesId = 2;
        }
        else if (Utility.Accesslevel != "Admin" && Utility.Accesslevel != "Operator")
        {
            MesId = 3;
        }
        else if (Utility.Accesslevel == "Admin" || Utility.Accesslevel == "Operator")
        {
            Timer1.Enabled = false;
            string Action = "Monitring Stopped";
            Utility.AuditLogger(Utility.UserName, Utility.Accesslevel, Action);
            //CustomValidator1_ServerValidate();

        }

    } //End of Stop Function

    protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (Timer1.Enabled == false) {
            CustomValidator1.ErrorMessage = "Application stopped monitoring Temperature and Humidity";
            args.IsValid = false;
        }
        if (Timer1.Enabled == true)
        {
            CustomValidator1.ErrorMessage = "Application started monitoring Temperature and Humidity";
            args.IsValid = false;
        }
        if (MesId == 1)
        {
            CustomValidator1.ErrorMessage = "Please Login to perform this operation";
        }
        if (MesId == 2)
        {
            CustomValidator1.ErrorMessage = "User Acccess Level could not be determined to perform this operation";
        }
        if (MesId == 3)
        {
            CustomValidator1.ErrorMessage = "Currently loggedin user does not permission to perform this operation";
        }
    }

    protected void TextBox1_TextChanged(object sender, EventArgs e)
    {

    }

    protected void Button2_Click1(object sender, EventArgs e)
    {
        Timer1.Enabled = false;
    }
}