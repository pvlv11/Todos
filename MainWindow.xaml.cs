using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Supabase;
using Supabase.Gotrue;
using Todos.Models;
using Todos;
using Todos.Properties;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;

namespace Todos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Supabase.Client client;
        private static String _url = Settings.Default.url;
        private static String _key = Settings.Default.apikey;
        private static ObservableCollection<String> _todos = new ObservableCollection<String>();
        IConfiguration Configuration { get; set; }

        public MainWindow()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<Todo>();
            Configuration = builder.Build();
            _url = Configuration["url"];
            _key = Configuration["apikey"];

            InitializeComponent();
        }

        private async void ButtonAddTodo_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtName.Text) && !lstTodos.Items.Contains(txtName.Text)) 
            { 
                _todos.Add(txtName.Text);
                await client.From<Models.Todo>().Insert(new Todo { Text = txtName.Text });  
                txtName.Clear();
                FetchTodos();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            lstTodos.ItemsSource = _todos;
            if (String.IsNullOrEmpty(_url))
            {
                ApiKeyDialogWindow modalWindow = new ApiKeyDialogWindow();
                modalWindow.ShowDialog();

                _key = ApiKeyDialogWindow.myApikey;
                _url = ApiKeyDialogWindow.myUrl;
                Configuration["apikey"] = _key;
                Configuration["url"] = _url;
            }

            if (String.IsNullOrEmpty(_key))
            {
                ApiKeyDialogWindow modalWindow = new ApiKeyDialogWindow();
                modalWindow.ShowDialog();

                _key = ApiKeyDialogWindow.myApikey;
                _url = ApiKeyDialogWindow.myUrl;
                Configuration["apikey"] = _key;
                Configuration["url"] = _url;
            }

            InitSupabase();
            FetchTodos();

        }

        private async void InitSupabase()
        {
            var options = new SupabaseOptions { AutoConnectRealtime = true };
            client = new Supabase.Client(_url, _key, options);
            await client.InitializeAsync();
        }

        public static async void FetchTodos()
        {
            try
            {
                var query = await client.From<Models.Todo>().Get();

                foreach (var todo in query.Models)
                {
                    if (!_todos.Contains(todo.Text))
                    {
                        _todos.Add(todo.Text);
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not fetch data, please try again.", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            ApiKeyDialogWindow modalWindow = new ApiKeyDialogWindow();
            modalWindow.ShowDialog();

            _key = ApiKeyDialogWindow.myApikey;
            Trace.WriteLine(_key);
        }



        private async void btnDeleteTodo_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button != null)
            {
                var todoText = button.DataContext.ToString();
                await client.From<Models.Todo>().Where(x => x.Text == todoText).Delete();
                _todos.Remove(todoText);
            }
            FetchTodos();
        }
    }
}
