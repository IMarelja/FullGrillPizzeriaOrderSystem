using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AdminUsersViewModel
    {
        public List<UserViewModelInfoForAdmin> Users { get; set; } = new();
    }
}
