using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QLSanBong.ViewModel;

namespace QLSanBong.View
{
    /// <summary>
    /// Interaction logic for QuanLiNguoiDung.xaml
    /// </summary>
    public partial class QuanLiNguoiDung : Window
    {
        public QuanLiNguoiDung()
        {
            InitializeComponent();
            DataContext = new QLSanBong.ViewModel.QuanLiNguoiDungViewModel();
        }

        private void pwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Đồng bộ PasswordBox với ViewModel theo đúng MVVM (one-way UI -> VM)
            if (DataContext is QuanLiNguoiDungViewModel vm && sender is PasswordBox pb)
            {
                vm.txtMatKhau = pb.Password;
            }
        }
    }
}
