﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServerDetector.Classes
{

    public static class Logging
    {
        private static object syncObj = new object();
        private static string setting_file_name = "log.txt";
        private static long log_file_max_size = 20 * 1024 * 1024;//20Mb
        public static void Log(string msg, Object obj = null)
        {
            string patc = getLogFileLocation();
            string msg_formated = formatMg(msg, obj);
            cheakLogFileSize();
            lock (syncObj)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(patc, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine(msg_formated);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static string formatMg(string msg, object obj)
        {
            string msg_ret = Environment.NewLine +
                DateTime.Now.ToString() +
                Environment.NewLine +
                "Message: " + msg;
            Exception ex = obj as Exception;
            if (ex != null)
            {
                msg_ret += Environment.NewLine +
                    "Error mssage: " + ex.Message + Environment.NewLine +
                    "Class: " + ex.Source + Environment.NewLine +
                    "Full : " + ex.ToString();
            }
            return msg_ret;
        }
        public static string getLogFileLocation()
        {
            try
            {
                string patch = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                patch += "\\" + setting_file_name;
                return patch;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "c:\\temp\\" + setting_file_name;
            }
        }

        private static void cheakLogFileSize()
        {
            try
            {
                string patch = getLogFileLocation();
                if (System.IO.File.Exists(patch))
                {
                    if (log_file_max_size < new System.IO.FileInfo(patch).Length)
                    {
                        lock (syncObj)
                        {
                            using (StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(patch, FileMode.Truncate, null)))
                            {
                                sw.WriteLine(patch, "Log file clean: " + DateTime.Now.ToLongDateString());
                                sw.Close();
                            }
                        }
                    }
                }

            }
            catch (Exception ex) 
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

    }
}
