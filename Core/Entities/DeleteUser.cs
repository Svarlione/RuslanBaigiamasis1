using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DeleteUser
    {
        public long DeletingUserId { get; set; }
        public long UserId { get; set; }
    }
}
