using App3.Models;
using Xamarin.Forms.Maps;

namespace App3.Maps
{

    public class PartyPin : Pin
    {
        public PartyPin() { }
        public PartyPin(Party p)
        {
            pid = p.pid;
            Type = PinType.Place;
            Position = p.geoPosition;
            Label = p.name;
            Address = p.address;
            Name = p.name;
        }
        public string Name { get; set; }
        public string pid { get; set; }
    }
}
