using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace QLSanBong
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Trỏ DataDirectory về thư mục project (2 cấp trên bin),
            // để EF dùng đúng cùng một file qlsanbong.mdf với Visual Studio
            try
            {
                string exeDir = AppDomain.CurrentDomain.BaseDirectory; // ...\bin\Debug\
                string projectDir = Path.GetFullPath(Path.Combine(exeDir, "..", ".."));
                AppDomain.CurrentDomain.SetData("DataDirectory", projectDir);
            }
            catch { }

            base.OnStartup(e);
        }
    }
}
