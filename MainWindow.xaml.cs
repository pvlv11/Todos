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

namespace Todos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String url = "https://tcgqpbvdhirptzglzlrj.supabase.co";
        private Supabase.Client client;
        private String _key;
        private ObservableCollection<String> _todos = new ObservableCollection<String>();


        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonAddTodo_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtName.Text) && !lstTodos.Items.Contains(txtName.Text)) 
            { 
                lstTodos.Items.Add(txtName.Text);
                await client.From<Models.Todo>().Insert(new Todo { Text = txtName.Text });  
                txtName.Clear();
                FetchTodos();
            }
        }

        private void GetApikey()
        {
            try
            {
                _key = Settings.Default.apikey;
            } 
            catch (ArgumentNullException ex) 
            {
                ApiKeyDialogWindow modalWindow = new ApiKeyDialogWindow();
                modalWindow.ShowDialog();

                _key = ApiKeyDialogWindow.myApikey;
                Settings.Default.apikey = _key;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lstTodos.ItemsSource = _todos;
            if (String.IsNullOrEmpty(Settings.Default.apikey))
            {
                ApiKeyDialogWindow modalWindow = new ApiKeyDialogWindow();
                modalWindow.ShowDialog();

                _key = ApiKeyDialogWindow.myApikey;
                Settings.Default.apikey = _key;
            }
            else
            {
                _key = Settings.Default.apikey;
            }

            InitSupabase();
            FetchTodos();

        }

        private async void InitSupabase()
        {
            var options = new SupabaseOptions { AutoConnectRealtime = true };
            client = new Supabase.Client(url, _key, options);
            await client.InitializeAsync();


        }

        private async void FetchTodos()
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

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            ApiKeyDialogWindow modalWindow = new ApiKeyDialogWindow();
            modalWindow.ShowDialog();

            _key = ApiKeyDialogWindow.myApikey;
            Trace.WriteLine(_key);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.apikey = _key;
            Properties.Settings.Default.Save();
        }

        private async void btnDeleteTodo_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button != null)
            {
                var todoText = button.DataContext.ToString();
                Trace.WriteLine(todoText);

                await client.From<Models.Todo>().Where(x => x.Text == todoText).Delete();
                _todos.Remove(todoText);
            }

            FetchTodos();
            Trace.WriteLine("todos fetched again");
        }
    }
}
