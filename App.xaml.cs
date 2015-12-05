using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GenerateProgrammeCode
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
        }

         //重写OnStartup，获得启动程序  
        protected override void OnStartup(StartupEventArgs e)  
        {  
            if (e.Args != null && e.Args.Count() > 0)  
            {  
                this.Properties["startexe"] = e.Args[0];  
                this.Properties["version"] = e.Args[1];
            }  
           base.OnStartup(e);  
        }    

    }
}
