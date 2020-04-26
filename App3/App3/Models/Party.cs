using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace App3.Models
{
    public class Party
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int maxInvites { get; set; }
        public bool going { get; set; }
        public List<Person> peopleGoing { get; set; }
        public List<Person> owners { get; set; }
        public DateTime time { get; set; }
        public string address { get; set; }
        public Position geoPosition { get; set; }

    }
}
