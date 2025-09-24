using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLSanBong.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private string _currentUser;
        public string CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); }
        }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set { _isAdmin = value; OnPropertyChanged(); }
        }

        public MainWindowViewModel()
        {
            // mặc định chưa đăng nhập
            CurrentUser = "";
            IsAdmin = false;
        }
    }
}
