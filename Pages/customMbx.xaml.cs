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
using System.Windows.Shapes;

namespace gymTracker.Pages
{
    /// <summary>
    /// Interaction logic for customMbx.xaml
    /// </summary>
    public partial class customMbx : Window
    {
        public customMbx(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
