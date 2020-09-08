using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace COREWEBSOCKET.Model
{
    public class TestUser
    {
        public bool IsActive;

        public string SubjectId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public List<Claim> Claims { get; set; }


     
    }


}
