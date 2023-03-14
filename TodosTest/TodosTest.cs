using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Supabase.Realtime.Constants;
using Todos;
using Todos.Models;
using System.Diagnostics;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Todos.TodosTest
{


    [TestClass]
    public class Client
    {
        private static Random random = new Random();
        private Supabase.Client ClientInstance;
        IConfiguration Configuration { get; set; }
        private static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [TestInitialize]
        public async Task InitializeTest()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<Client>();
            Configuration = builder.Build();

            var url = Configuration["url"];
            var apikey = Configuration["apikey"];

            ClientInstance = new Supabase.Client(url, apikey, new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true,
            });

            await ClientInstance.InitializeAsync();
        }

        [TestMethod("Client: Initializes")]
        public void ClientInitialize()
        {
            Assert.IsNotNull(ClientInstance.Realtime);
            Assert.IsNotNull(ClientInstance.Auth);
        }

        [TestMethod("Client: Connects to Realtime")]
        public async Task ClientConnectsToRealtime()
        {
            var tsc = new TaskCompletionSource<bool>();

            var email = $"{RandomString(10)}@igoreq.pl";
            await ClientInstance.Auth.SignUp(email, RandomString(10));

            var channel = ClientInstance.Realtime.Channel("realtime", "public", "channels");

            channel.StateChanged += (sender, ev) =>
            {
                if (ev.State == ChannelState.Joined) tsc.SetResult(true);
            };

            await channel.Subscribe();

            var result = await tsc.Task;
            Assert.IsTrue(result);
        }

        [TestMethod("SupabaseModel: Succesfully Updates")]
        public async Task SupabaseModelUpdates()
        {
            var todo = new Todo { Text = $"{RandomString(10)}" };
            var insertResult = await ClientInstance.From<Todos.Models.Todo>().Insert(todo);

            var newTodo = insertResult.Models.FirstOrDefault();

            var newText = $"Text updated at {DateTime.Now.ToLocalTime()}";
            newTodo.Text = newText;

            var updatedResult = await ClientInstance.From<Todo>().Where(x => x.Text == todo.Text).Set(x => x.Text, newText).Update();

            Assert.AreEqual(newText, updatedResult.Models.First().Text);
        }

        [TestMethod("SupabaseModel: Succesfuly Deletes")]
        public async Task SupaboseModelDeletes()
        {
            var todo = new Todo { Text = $"{RandomString(10)}" };
            var count = await ClientInstance.From<Todo>().Select("*").Count(Postgrest.Constants.CountType.Exact);

            await ClientInstance.From<Todo>().Insert(todo);
            await ClientInstance.From<Todo>().Where(x => x.Text == todo.Text).Delete();
            
            var newCount = await ClientInstance.From<Todo>().Select("*").Count(Postgrest.Constants.CountType.Exact);

            Assert.AreEqual(count, newCount);
        }
    }
}
