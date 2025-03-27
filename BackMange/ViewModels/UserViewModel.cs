using BackMange.DTO;
using BackMange.Models;

namespace BackMange.ViewModels
{
    public class UserViewModel
    {
        public CUserDTO User { get; set; }
        public List<TUserType> AvailableUserTypes { get; set; }
        public List<int> SelectedUserTypes { get; set; }
    }
}
