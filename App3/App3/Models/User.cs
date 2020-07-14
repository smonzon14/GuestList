using System.Diagnostics;


namespace App3.Models
{
    public class User
    {
        public User() { }
        public string uid { get; set; }
        public string bio { get; set; }
        public int status { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public int friendStatus { get; set; }

        public void printUser()
        {
            Debug.WriteLine("id: " + uid);
            Debug.WriteLine("bio: " + bio);
            Debug.WriteLine("status: " + status.ToString());
            Debug.WriteLine("name: " + name);
            Debug.WriteLine("email: " + email);
        }
    }
}
