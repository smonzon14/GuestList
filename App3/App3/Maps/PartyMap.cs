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

        public void generateMap(List<Party> pList)
        {
            foreach (Party p in pList)
            {
                int index = -1;
                // Update existing pin
                if ((index = partyPins.FindIndex(existingPin => existingPin.partyId == p.id)) > -1)
                {
                    partyPins[index].Position = p.geoPosition;
                    partyPins[index].Label = p.description;
                    partyPins[index].Address = p.address;
                    partyPins[index].Name = p.name;
                }
                else // Create new pin
                {
                    PartyPin pin = new PartyPin
                    {
                        partyId = p.id,
                        Type = PinType.Place,
                        Position = p.geoPosition,
                        Label = p.description,
                        Address = p.address,
                        Name = p.name,
                    };

                    Pins.Add(pin);
                    partyPins.Add(pin);
                }
            }



            MoveToRegion(MapSpan.FromCenterAndRadius(Pins[0].Position, Distance.FromMiles(0.3)));
        }
    }
}
