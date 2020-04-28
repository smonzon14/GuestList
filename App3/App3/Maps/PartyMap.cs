using App3.Maps;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;
using Xamarin.Forms;

namespace App3.Models
{
    public class PartyMap : Map
    {
        public event EventHandler CallToNativeMethod;

        public void RaiseCallToNativeMethod(PartyPin pin)
        {
            selectedPin = pin;
            if (pin == null)
            {
                Console.WriteLine("no such pin");
                return;
            }
            CallToNativeMethod?.Invoke(this, new EventArgs());
            // C# 6 way (Does the same thing as the line above, but looks cleaner)
            // CallToNativeMethod?.Invoke(this, new EventArgs());
        }
        public PartyPin selectedPin { get; set; }
        public PartyMap()
        {
            partyPins = new List<PartyPin>();
        }
        public List<PartyPin> partyPins { get; set; }

    }
}
