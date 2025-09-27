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
    /// Interaction logic for dangnhap.xaml
    /// </summary>
    public partial class dangnhap : Window
    {
        public dangnhap()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void btnDangNhap_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DangNhapViewModel;

            if (vm != null)
            {
                string matKhau = passwordBox.Password; // đảm bảo lấy đúng
                
                // Hiển thị thông tin debug
                debugInfo.Text = $"Đang đăng nhập với: {vm.TenDangNhap} / {matKhau}";
                
                vm.DangNhap(matKhau);
            }
        }

    }
}
