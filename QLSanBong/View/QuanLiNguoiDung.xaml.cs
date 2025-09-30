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

        private void btnQuayVe_Click(object sender, RoutedEventArgs e)
        {
            // Quay về MainWindow
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void btnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Xác nhận đăng xuất
                var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", 
                    "Xác nhận đăng xuất", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Xóa thông tin user hiện tại
                    QLSanBong.Model.CurrentUser.User = null;
                    
                    // Mở form đăng nhập
                    var loginWindow = new dangnhap();
                    loginWindow.Show();
                    
                    // Đóng form hiện tại
                    this.Close();
                    
                    // Hiển thị thông báo
                    MessageBox.Show("Đã đăng xuất thành công!", "Thông báo", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng xuất: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
