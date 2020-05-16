using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace App3.Models
{
    public class Comment
    {
        public string username;
        public Image profileImage;
        public string comment;
        public DateTime datePosted;
        public int likes;
        public List<Comment> subComments;
    }
}
