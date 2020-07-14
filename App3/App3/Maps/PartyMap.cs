
using System;
using System.Collections.Generic;
using Xamarin.Forms.GoogleMaps;

namespace App3.Models
{
    public class PartyMap : Map
    {
        public event EventHandler CallToNativeMethod;
        
        public void RaiseCallToNativeMethod()
        {
            CallToNativeMethod?.Invoke(this, new EventArgs());
        }
        public Pin selectedPin { get; set; }

        public PartyMap()
        {
            //IsShowingUser = true;
            MapType = MapType.Street;
        }
        public void generateMap(List<Party> pList)
        {
            Pins.Clear();
            if (pList.Count > 0)
            {
                
                foreach (Party p in pList)
                {
                    Pins.Add(p.pin);

                    
                }

                moveTo(pList[0]);
            }

        }
        public void moveTo(Party party)
        {
            if (party == null) return;
            MapSpan span = MapSpan.FromCenterAndRadius(party.geoPosition, Distance.FromMiles(0.12));
            selectedPin = party.pin;

            RaiseCallToNativeMethod();
            MoveToRegion(span);

        }

    }
}
