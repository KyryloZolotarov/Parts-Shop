﻿namespace Order.Hosts.Models.Requests
{
    public class AddUserRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
