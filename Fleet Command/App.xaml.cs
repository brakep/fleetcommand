using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Remoting;
using Microsoft.Shell;
using Fleet_Command;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "My_Unique_Application_String";
        public string SecretKey;
        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }


        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            //throw new NotImplementedException();
            foreach (var arg in args)
            {
                if (arg != args.First())
                {
                    (MainWindow as MainWindow).Keymaster = arg;
                    //SecretKey = arg;
                    //Application.Current.MainWindow.textBox = arg;
                    //MessageBox.Show(arg);
                }
            }
            return true;
        }
        #endregion
    }
}
