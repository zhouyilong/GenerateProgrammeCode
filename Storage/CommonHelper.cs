using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GenerateProgrammeCode.Storage
{
    public class CommonHelper
    {

    }

    /// <summary>
    /// <para>日志帮助类</para>
    /// <para>by zhouyilong 20151214</para>
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// <para>写入日志</para>
        /// <para>by zhouyilong 20151214</para>
        /// </summary>
        /// <param name="str">需要写入的字符</param>
        public static void WriteLog(string str)
        {

            string strLog = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + str + "/r/n";

            StreamWriter errorlog = new StreamWriter(System.IO.Path.Combine(Environment.CurrentDirectory, @"log.txt"), true);
            errorlog.WriteLine(strLog);
            errorlog.Flush();
            errorlog.Close();
        }
    }
}
