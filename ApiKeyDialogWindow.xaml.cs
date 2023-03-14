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
using Todos;

namespace Todos
{
    /// <summary>
    /// Interaction logic for ApiKeyDialogWindow.xaml
    /// </summary>
    public partial class ApiKeyDialogWindow : Window
    {
        public static string myApikey;
        public static string myUrl;
        public ApiKeyDialogWindow()
        {
            InitializeComponent();
        }

        private void btnSaveApikey_Click(object sender, RoutedEventArgs e)
        {
            Todos.MainWindow._key = passwordBoxApikey.Password;
            Todos.MainWindow._url = textBoxUrl.Text;
            this.Close();
            MainWindow.FetchTodos();
        }
    }
}
