using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS_499_Project.Object_Classes
{
    public class AdminProfile : ProfileInterface
{

        public AdminProfile()
        {
            this.profile_type = ProfileType.ADMIN;
        }
}
}
