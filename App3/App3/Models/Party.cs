using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;
using Xamarin.Forms;

namespace App3.Models
{

    public class Party
    {
        static Random rand = new Random();
        public static List<string> GetRandomHexColor()
        {
            int hue = rand.Next(0, 255);
            Color c1 = Color.FromHsla(
                (hue / 255.0f),
                0.7f,
                0.5f);

            hue -= 50;
            if (hue < 0) hue += 255;

            Color c2 = Color.FromHsla(
                (hue / 255.0f),
                0.7f,
                0.5f, 0.75);
            return new List<string> { c1.ToHex(), c2.ToHex() };
        }
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool going { get; set; }
        public List<Person> peopleGoing { get; set; }
        public int numPeopleGoing
        {
            get; set;
        }
        public Person thrower { get; set; }
        public DateTime time { get; set; }
        public string address { get; set; }
        public Position geoPosition { get; set; }
        public string primaryHexColor { get
            {
                if (primaryHexColor == null)
                {
                    var colors = GetRandomHexColor();
                    primaryHexColor = colors[0];
                    secondaryHexColor = colors[1];
                }
                return primaryHexColor;
            }
            set
            {
                primaryHexColor = value;
            }
        }
        public string secondaryHexColor { get; set; }
        public List<Comment> comments { get; set; }
        
    }
}
