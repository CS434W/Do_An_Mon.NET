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
using System.Windows.Navigation;
using System.Windows.Shapes;
using QLSanBong.Model;

namespace QLSanBong.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Ẩn nút đăng nhập nếu đã có người dùng đăng nhập
            if (CurrentUser.User != null)
            {
                btnDangNhap.Visibility = Visibility.Collapsed;
            }
        }

        private void btnDangNhap_Click(object sender, RoutedEventArgs e)
        {
            var login = new dangnhap();
            login.Show();
            Close();
        }

        private void FeatureButton_Click(object sender, RoutedEventArgs e)
        {
            // Nếu chưa đăng nhập, điều hướng về trang đăng nhập và đóng MainWindow
            if (CurrentUser.User == null)
            {
                var loginWindow = new dangnhap();
                loginWindow.Show();
                Close();
                return;
            }

            // Đã đăng nhập: điều hướng tới trang tương ứng
            var button = sender as Button;
            if (button == null) return;

            if (button.Name == "btnSanBong")
            {
                var qlsb = new QuanLySanBong();
                qlsb.Show();
                return;
            }

            if (button.Name == "btnNguoiDung")
            {
                var qlnd = new QuanLiNguoiDung();
                qlnd.Show();
                return;
            }

            MessageBox.Show("Chức năng đang phát triển");
        }
    }
}
