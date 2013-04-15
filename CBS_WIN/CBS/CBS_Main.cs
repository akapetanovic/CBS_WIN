﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace CBS
{
    class CBS_Main
    {
        private static bool FileWatcherEnabled = true;
        // WINDOWS
        //
        // Will get changed to LINUX paths on Initialise if APP is running
        // on LINUX !!!
        private static string Source_Path = @"C:\CBS\source\";
        private static string Destination_Path = @"C:\CBS\destination\";
        private static string App_Settings_Path = @"C:\CBS\settings\";

        // Common
        private static string HEART_BEAT = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;

        // Timer to save off last time application was alive
        private static System.Timers.Timer HEART_BEAT_TIMER;

        public enum Host_OS { WIN, LINUX };
        public static Host_OS Get_Host_OS()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                return Host_OS.LINUX;
            else
                return Host_OS.WIN;
        }

        public static void SetSourceAndDestinationPaths(string SOURCE, string DESTINATION)
        {
            Source_Path = SOURCE;
            Destination_Path = DESTINATION;
        }

        // Returns power off time (+/- 59 sec)
        public static DateTime Get_Power_OFF_Time()
        {
            return GetDate_Time_From_YYYYMMDDHHMMSS(HEART_BEAT);
        }

        public static DateTime GetDate_Time_From_YYYYMMDDHHMMSS(string DATETIME)
        {
            int Year = int.Parse(DATETIME.Substring(0, 4));
            int Month = int.Parse(DATETIME.Substring(4, 2));
            int Day = int.Parse(DATETIME.Substring(6, 2));
            int Hour = int.Parse(DATETIME.Substring(8, 2));
            int Minute = int.Parse(DATETIME.Substring(10, 2));
            int Sec = int.Parse(DATETIME.Substring(12, 2));
            return new DateTime(Year, Month, Day, Hour, Minute, Sec);
        }

        public static string GetDate_Time_AS_YYYYMMDDHHMMSS(DateTime Time_In)
        {
            return Time_In.Year.ToString("0000") + Time_In.Month.ToString("00") + Time_In.Day.ToString("00") + Time_In.Hour.ToString("00") + Time_In.Minute.ToString("00") + Time_In.Second.ToString("00");
        }

        public static string Get_DATE_AS_YYMMDD(DateTime Time_In)
        {
            return Time_In.Year.ToString("00") + Time_In.Month.ToString("00") + Time_In.Day.ToString("00");
        }

        public static string Get_TIME_AS_HHMM(DateTime Time_In)
        {
            return Time_In.Hour.ToString("00") + Time_In.Minute.ToString("00");
        }

        public static string Get_Source_Dir()
        {

            return Source_Path;

        }

        public static string Get_Destination_Dir()
        {

            return Destination_Path;

        }

        public static string Get_APP_Settings_Path()
        {
            return App_Settings_Path;
        }

        public static void Initialize()
        {
            /////////////////////////////////////////////////////////////////
            // First check where we are running and prepare app
            //
            if (Get_Host_OS() == Host_OS.LINUX)
            {
                // Linux
                Source_Path = "/var/EFD/";
                Destination_Path = "/var/cbs/prediction/flights/";
                App_Settings_Path = "/var/cbs/settings/";
            }

            // Now make sure that proper directory structure 
            // is set up on the host machine
            if (Directory.Exists(Source_Path) == false)
                Directory.CreateDirectory(Source_Path);
            if (Directory.Exists(Destination_Path) == false)
                Directory.CreateDirectory(Destination_Path);
            if (Directory.Exists(App_Settings_Path) == false)
                Directory.CreateDirectory(App_Settings_Path);

            // Check if cbs_config.txt exists, if so load settings
            // data saved from the previous session
            string Settings_Data;
            string FileName = Path.Combine(App_Settings_Path, "cbs_config.txt");
            char[] delimiterChars = { ' ' };
            StreamReader MyStreamReader;
            if (File.Exists(FileName))
            {
                // Lets read in settings from the file
                MyStreamReader = System.IO.File.OpenText(FileName);
                while (MyStreamReader.Peek() >= 0)
                {
                    Settings_Data = MyStreamReader.ReadLine();
                    string[] words = Settings_Data.Split(delimiterChars);

                    switch (words[0])
                    {
                        case "SOURCE_DIR":

                            Source_Path = words[1];

                            break;
                        case "DESTINATION_DIR":

                            Destination_Path = words[1];

                            break;
                        case "HEART_BEAT":
                            HEART_BEAT = words[1];
                            break;
                    }
                }

                MyStreamReader.Close();
                MyStreamReader.Dispose();

                // Here check if there has been more 10min since application has been down
                TimeSpan TenMin = new TimeSpan(0, 10, 0);
                TimeSpan AppDown = DateTime.Now - Get_Power_OFF_Time();

                if (AppDown > TenMin)
                    ClearSourceDirectory();

                // Lets save once so HART BEAT gets saved right away
                SaveSettings();
            }
            else
            {
                // Lets then create one with default setting
                SaveSettings();

                // Since we had no idea when the application was last powered off
                // we assume it has been more than timuout parameter, sop lets delete all files
                ClearSourceDirectory();
            }

            // Now start heart beat timer.
            HEART_BEAT_TIMER = new System.Timers.Timer(10000); // Set up the timer for 1minute
            HEART_BEAT_TIMER.Elapsed += new ElapsedEventHandler(_HEART_BEAT_timer_Elapsed);
            HEART_BEAT_TIMER.Enabled = true;

            // Finally start file watcher to process incomming data
            if (FileWatcherEnabled)
                FileWatcher.CreateWatcher(Source_Path);
        }

        public static void Restart_Watcher()
        {
            if (FileWatcherEnabled)
            {
                FileWatcher.StopWatcher();
                FileWatcher.CreateWatcher(Source_Path);
            }
        }

        private static void _HEART_BEAT_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SaveSettings();
        }

        // Deletes all files from the source directory
        private static void ClearSourceDirectory()
        {
            foreach (string file in Directory.GetFiles(Source_Path))
                File.Delete(file);
        }

        public static void SaveSettings()
        {
            string FileName = Path.Combine(App_Settings_Path, "cbs_config.txt");
            string Settings_Data = "";

            //////////////////////////////////////////////////////////////////////////////////////
            // Do not chanage the order of calls

            Settings_Data = Settings_Data + "SOURCE_DIR" + " " + Source_Path + Environment.NewLine;
            Settings_Data = Settings_Data + "DESTINATION_DIR" + " " + Destination_Path + Environment.NewLine;


            Settings_Data = Settings_Data + "HEART_BEAT" + " " + GetDate_Time_AS_YYYYMMDDHHMMSS(DateTime.Now) + Environment.NewLine;
            //////////////////////////////////////////////////////////////////////////////////////

            // create a writer and open the file
            TextWriter tw = new StreamWriter(FileName);

            try
            {
                // write a line of text to the file
                tw.Write(Settings_Data);
            }
            catch
            {
            }

            // close the stream
            tw.Close();
            tw.Dispose();
        }
    }
}