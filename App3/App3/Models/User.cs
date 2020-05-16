
using System.Diagnostics;


namespace App3.Models
{
    public class User : Person
    {
        public User() { }
        public string sub { get; set; }
        public string email { get; set; }
        public string id_token { get; set; }
        public string access_token { get; set; }
        public string password { get; set; }
        public void printUser()
        {
            Debug.WriteLine("id: " + id.ToString());
            Debug.WriteLine("sub: " + sub);
            Debug.WriteLine("bio: " + bio);
            Debug.WriteLine("status: " + status.ToString());
            Debug.WriteLine("name: " + name);
            Debug.WriteLine("email: " + email);
            Debug.WriteLine("id_token: " + id_token);
            Debug.WriteLine("access_token: " + access_token);
        }
    }
}
