using App3.Maps;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Linq;

namespace App3.Models
{
    public class PartyMap : Map
    {
        public event EventHandler CallToNativeMethod;

        public void RaiseCallToNativeMethod()
        {
            CallToNativeMethod?.Invoke(this, new EventArgs());
        }
        public PartyPin selectedPin { get; set; }

        public List<PartyPin> partyPins { get; set; } = new List<PartyPin>();
        public PartyMap()
        {
            //IsShowingUser = true;
            MapType = MapType.Street;
        }
        public void generateMap(List<Party> pList)
        {
            
            if (pList.Count > 0)
            {
                foreach (Party p in pList)
                {
                    int index;
                    // Update existing pin
                    if ((index = partyPins.FindIndex(existingPin => existingPin.pid.Equals(p.pid))) > -1)
                    {
                        var updatedPin = new PartyPin(p);
                        partyPins[index] = updatedPin;
                        Pins[index] = updatedPin;
                    }
                    else // Create new pin
                    {
                        PartyPin pin = new PartyPin(p);
                        Pins.Add(pin);
                        partyPins.Add(pin);
                        
                    }
                }

                moveTo(pList[0]);
            }
            
        }
        public void moveTo(Party party)
        {
            if (party == null) return;
            MapSpan span = MapSpan.FromCenterAndRadius(party.geoPosition, Distance.FromMiles(0.12));
            selectedPin = partyPins.Find(x => x.pid.Equals(party.pid));

            RaiseCallToNativeMethod();
            MoveToRegion(span);
            
        }
        
    }
}
