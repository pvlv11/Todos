using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Postgrest;
using Supabase;
using System.Net;


namespace Todos.Services
{
    public class DatabaseService
    {
        private readonly Supabase.Client client;

        public DatabaseService(Supabase.Client client)
        {
            this.client = client;
        }


    }
}
