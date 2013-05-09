using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CBS
{
    class EFD_Trajectory
    {
        // EFD_Trajetory_DATETIME.kml

            //<?xml version="1.0" encoding="UTF-8"?>
            //<kml xmlns="http://www.opengis.net/kml/2.2">
            //<Document>
            //<Placemark>
            //    <name>DLH2394 Trajectory</name>
            //<TimeStamp>
            //    <when>2013-02-20T00:05:20Z</when>
            //</TimeStamp>
            //<ExtendedData>
            //    <Data name="dataSourceName">
            //         <value>EFD</value>
            //      </Data>
            //    <Data name="markerType">
            //        <value>polyline</value>
            //    </Data>
            //    <Data name="lineColor">
            //        <value>ffff00</value>
            //    </Data>
            //    <Data name="popupLine1">
            //        <value>Callsign,ADEP,ADES</value>
            //      </Data>
            //      <Data name="popupLine2">
            //            <value>EOBD, EOBT</value>
            //      </Data>
            //      <Data name="popupLine3">
            //            <value>IFPLID</value>
            //      </Data>
            //      <Data name="fileLocation">
            //            <value>flights/ACID_IFPLID_DATETIME/EFD/EFD_Trajectory_DATETIME.kml</value>
            //      </Data>
            //</ExtendedData>
            //     <LineString>
            //            <coordinates>
            //            12.17152,51.41049,646,20130305003800
            //            12.09607,51.41915,1201,20130305003900
            //            12.05830,51.48931,1762,20130305004000
            //        </coordinates>
            //      </LineString>
            //  </Placemark>
            //</Document>
            //</kml>
        public static void Generate_Output (EFD_Msg Message_Data)
        {
            string Time_Stamp = KML_Common.Get_KML_Time_Stamp();
            string KML_File_Content =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine +
                    "<kml xmlns=\"http://www.opengis.net/kml/2.2\">" + Environment.NewLine +
                    "<Document>" + Environment.NewLine;
               
            // Get the final data path
            string File_Path = Get_Dir_By_ACID_AND_IFPLID(Message_Data.ACID, Message_Data.IFPLID);
            File_Path = Path.Combine(File_Path, ("EFD_Trajectory_" + CBS_Main.GetDate_Time_AS_YYYYMMDDHHMMSS(DateTime.UtcNow) + ".kml"));

            // Save data in the tmp directory first
            string Tmp = Path.Combine(CBS_Main.Get_Temp_Dir(), ("EFD_Trajectory_" + CBS_Main.GetDate_Time_AS_YYYYMMDDHHMMSS(DateTime.UtcNow) + ".kml"));

            // create a writer and open the file
            TextWriter tw = new StreamWriter(Tmp);

            try
            {
                // write a line of text to the file
                tw.Write(KML_File_Content);
            }
            catch
            {

            }

            // close the stream
            tw.Close();

            // Now move it to the final destination
            File.Move(Tmp, File_Path);
        }

        public static string Get_Dir_By_ACID_AND_IFPLID(string ACID, string IFPLID)
        {
            string DIR = "";
            // First check if directory already exists
            string IFPLID_DIR_NAME = ACID + "_" + IFPLID + "_*";
            string[] DestDirectory = Directory.GetDirectories(CBS_Main.Get_Destination_Dir(), IFPLID_DIR_NAME);
            if (DestDirectory.Length == 1)
            {
                DIR = DestDirectory[0];
                DIR = Path.Combine(DIR, "EFD");
            }
            return DIR;
        }
    }
}
