using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Postgrest.Attributes;
using Postgrest.Models;
using Supabase;

namespace Todos.Models
{
    [Table("todos")]
    public class Todo : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("created_at", IgnoreOnInsert = true, IgnoreOnUpdate = true)]
        public DateTime CreatedAt { get; set;}

        [Column("todo")]
        public string? Text { get; set; }
    }
}
