using System;
using System.Collections.Generic;

#nullable disable

namespace Domain.DbModels
{
    public partial class TbUser
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public DateTime Created { get; set; }
        public string Salt { get; set; }
    }
}
