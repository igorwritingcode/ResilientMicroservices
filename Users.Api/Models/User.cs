﻿namespace Users.Api.Models
{
    public class User
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public Address Address { get; set; }
    }
}
