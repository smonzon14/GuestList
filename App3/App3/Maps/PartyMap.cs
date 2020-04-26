using App3.Maps;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace App3.Models
{
    public class PartyMap : Map
    {
        public PartyMap()
        {
            partyPins = new List<PartyPin>();
        }

        public List<PartyPin> partyPins { get; set; }

    }
}
