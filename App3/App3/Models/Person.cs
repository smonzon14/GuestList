using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace App3.Models
{
    public abstract class Person
    {

        public string uid { get; set; }
        //public Token token { get; set; }
        public string bio { get; set; }
        public int status { get; set; }
        public string name { get; set; }
        //public List<Party> parties { get; set; }
        //public List<Person> friends { get; set; }
    }
}
