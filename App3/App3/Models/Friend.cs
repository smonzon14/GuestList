using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace App3.Models
{
    class Friend : Person
    {

        public Image image { get; set; }
        public string activity { get
            {
                switch (status)
                {
                    case 1:
                        return "Going Out";
                    case 2:
                        return "Staying In";
                    default:
                        return "Offline";
                }

            } }
    }
}
