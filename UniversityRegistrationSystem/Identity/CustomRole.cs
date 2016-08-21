using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityRegistrationSystem.Identity
{
    public class CustomRole : IRole
    {
        public CustomRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public CustomRole(string name)
            : this()
        {
            this.Name = name;
        }

        public CustomRole(string name, string id)
        {
            this.Name = name;
            this.Id = id;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}