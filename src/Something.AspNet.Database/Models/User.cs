using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Something.AspNet.Database.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
    }
}