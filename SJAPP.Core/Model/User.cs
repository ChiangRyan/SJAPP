using System.Collections.Generic;

namespace SJAPP.Core.Model
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Permissions { get; set; }
    }
}