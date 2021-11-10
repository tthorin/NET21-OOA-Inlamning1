namespace OOA_Inlamning1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Person
    {
        public int id { get; set; }
        public string first_name { get; set; } = "";
        public string last_name { get; set; } = "";
        public string email { get; set; } = "";
        public string username { get; set; } = "";
        public string password { get; set; } = "";
        public string country { get; set; } = "";
        public string FullName
        {
            get { return first_name + " " + last_name; }
        }

    }
}
