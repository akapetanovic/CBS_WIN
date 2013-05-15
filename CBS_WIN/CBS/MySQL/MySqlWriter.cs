using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace CBS
{
    public static class MySqlWriter
    {
        private static string connString;
        private static MySqlConnection MySQLconn;

        public static class MySQLConnetionString
        {
            public static string login_name = "root";
            public static string server_name = "localhost";
            public static string database = "flights";
        }

        public static void CloseConnection()
        {
            if (MySQLconn.State == System.Data.ConnectionState.Open)
                MySQLconn.Close();
        }

        // UNIQUE_ID
        // IFPLID 
        // ARCID 
        // FLTSTATE  
        // ADEP 
        // ADES 
        // ARCTYP 
        // ETI 
        // XTI 
        // LASTUPD 
        // ENTRIES        
        public static void Write_One_Message(EFD_Msg Message)
        {
            // Lets build SEQMUAC string
            // FORMAT:
            // //RH1,1515,1522//RH2,1515,1522
            string SEQMUAC = "";
            foreach (EFD_Msg.Sector_Type Msg in Message.Sector_List)
            {
                SEQMUAC = SEQMUAC + "//" + Msg.ID + ',' + GetTimeAS_HHMM(Msg.SECTOR_ENTRY_TIME) + ',' + GetTimeAS_HHMM(Msg.SECTOR_EXIT_TIME);
            }

            // LASTUPD
            DateTime T_Now = DateTime.UtcNow;
            string LASTUPD = T_Now.Year.ToString("0000") + T_Now.Month.ToString("00") + T_Now.Day.ToString("00") +
                T_Now.Hour.ToString("00") + T_Now.Minute.ToString("00") + T_Now.Second.ToString("00");

            string UNIQUE_ID = Message.IFPLID + '_' + Message.ACID;

            string query = "INSERT INTO " + "tms1" +
                " (UNIQUE_ID, IFPLID, ARCID, FLTSTATE, ADEP, ADES, ARCTYP, ETI, XTI, LASTUPD, ENTRIES) " +
                " VALUES (" + Get_With_Quitation(UNIQUE_ID) + "," +
                             Get_With_Quitation(Message.IFPLID) + "," +
                             Get_With_Quitation(Message.ACID) + "," +
                             Get_With_Quitation(Message.FLTSTATE) + "," +
                             Get_With_Quitation(Message.ADEP) + "," +
                             Get_With_Quitation(Message.ADES) + "," +
                             Get_With_Quitation(Message.ARCTYP) + "," +
                             Get_With_Quitation(CBS_Main.GetDate_Time_AS_YYYYMMDDHHMMSS(Message.ENTRY_AOI_TIME)) + "," +
                             Get_With_Quitation(CBS_Main.GetDate_Time_AS_YYYYMMDDHHMMSS(Message.EXIT_AOI_TIME)) + "," +
                             Get_With_Quitation(LASTUPD) + "," +
                             Get_With_Quitation(SEQMUAC) + ")";

            // Make sure connection is opened
            if (MySQLconn.State == System.Data.ConnectionState.Open)
            {
                
                // First delete the data for the flight (if already exists)
                string delete_query = "DELETE FROM " + "tms1" + " WHERE UNIQUE_ID= " + Get_With_Quitation(UNIQUE_ID);
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(delete_query, MySQLconn);

                try
                {
                    //Execute command
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //create command and assign the query and connection from the constructor
                cmd = new MySqlCommand(query, MySQLconn);

                try
                {
                    //Execute command
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("MySQL connection down...not able to update table");
            }
        }

        private static string Get_With_Quitation(string String_IN)
        {
            return "'" + String_IN + "'";
        }

        private static string GetTimeAS_HHMM(DateTime Time_In)
        {
            return Time_In.Hour.ToString("00") + Time_In.Minute.ToString("00");
        }

        public static void Initialise()
        {
            // Set the connection string
            connString = "server=" + MySQLConnetionString.server_name +
               ";User Id=" + MySQLConnetionString.login_name + ";database=" + MySQLConnetionString.database;

            // Open up the connection
            MySQLconn = new MySqlConnection(connString);

            try
            {
                MySQLconn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
