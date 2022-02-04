using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp10
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public class helper
    {
        public static bool flag = false;
        public static int prioritet = 0;
        public static WpfApp10Entities ent;
        public static WpfApp10Entities GetContext()
        {
            if (ent == null)
            {
                ent = new WpfApp10Entities();
            }
            return ent;
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            frame.Content = new ListAgent(frame);
        }

        private void frame_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                ListAgent pg = (ListAgent)e.Content;
                pg.Load();
            }
            catch { };

        }
    }
}
