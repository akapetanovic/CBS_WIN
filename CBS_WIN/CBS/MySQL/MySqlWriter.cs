using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;

namespace CBS
{
    public static class MySqlWriter
    {
        private static string connString;

        // IFPLID 
        // ARCID 
        // FLTSTATE  
        // ADEP 
        // ADES 
        // ARCTYP 
        // ETI 
        // XTI 
        // LASTUPD 
        // SEQMUAC        
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
            string LASTUPD = T_Now.Year.ToString("0000") + '-' + T_Now.Month.ToString("00") + '-' + T_Now.Day.ToString("00") +
                T_Now.Hour.ToString("00") + T_Now.Minute.ToString("00") + T_Now.Second.ToString("00");


        }

        private static string GetTimeAS_HHMM(DateTime Time_In)
        {
            return Time_In.Hour.ToString("00") + Time_In.Minute.ToString("00");
        }

        public static void Initialise()
        {
            // Set the connection string
            connString = "server=" + "Server" +
               ";User Id=" + "root" + ";database=" + "database";

            // Open up the connection
        }
    }
}
