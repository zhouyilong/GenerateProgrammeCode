using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GenerateProgrammeCode.Storage
{
    public class CommonHelper
    {
        /// <summary>
        /// <para>下载文件的内容</para>
        /// <para>by zhouyilong 20160415</para>
        /// </summary>
        /// <returns>文件中的数据</returns>
        public static string LoadFileText(string filePath, Encoding encoding)
        {
            string result = string.Empty;
            FileStream fs;
            if (File.Exists(filePath))
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using (fs)
                {

                    StreamReader streamReader = new StreamReader(fs, encoding);

                    result = streamReader.ReadToEnd();
                    streamReader.Close();
                    streamReader.Dispose();
                }

            }

            return result;
        }
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
