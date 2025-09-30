using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            UpdateLoginUI();
            LoadDashboardData();
        }

        private void UpdateLoginUI()
        {
            if (CurrentUser.User != null)
            {
                // Đã đăng nhập: ẩn nút đăng nhập, hiện nút đăng xuất và thông tin user
                btnDangNhap.Visibility = Visibility.Collapsed;
                btnDangXuat.Visibility = Visibility.Visible;
                userInfoPanel.Visibility = Visibility.Visible;
                
                // Hiển thị thông tin người dùng
                string userInfo = $"Xin chào: {CurrentUser.User.TenDangNhap} ({CurrentUser.User.VaiTro})";
                txtUserInfo.Text = userInfo;
            }
            else
            {
                // Chưa đăng nhập: hiện nút đăng nhập, ẩn nút đăng xuất và thông tin user
                btnDangNhap.Visibility = Visibility.Visible;
                btnDangXuat.Visibility = Visibility.Collapsed;
                userInfoPanel.Visibility = Visibility.Collapsed;
                txtUserInfo.Text = "";
            }
        }

        private void LoadDashboardData()
        {
            try
            {
                using (var db = new Entities1())
                {
                    DateTime today = DateTime.Today;

                    // 1. Hôm nay: số đặt sân
                    int todayBookings = db.LICH_DAT_SAN
                        .Count(x => x.ThoiGianBatDau.HasValue &&
                                   x.ThoiGianBatDau.Value.Year == today.Year &&
                                   x.ThoiGianBatDau.Value.Month == today.Month &&
                                   x.ThoiGianBatDau.Value.Day == today.Day);

                    // 2. Tổng khách hàng
                    int totalCustomers = db.KHACH_HANG.Count();

                    // 3. Doanh thu
                    decimal revenue = db.THANH_TOAN.Sum(x => (decimal?)x.SoTien) ?? 0;

                    // 4. Sân trống
                    int totalFields = db.SAN_BONG.Count();
                    var bookedFields = db.LICH_DAT_SAN
        .Where(x => x.ThoiGianBatDau.HasValue &&
                    DbFunctions.TruncateTime(x.ThoiGianBatDau.Value) == today.Date)
        .Select(x => x.MaSan)
        .Distinct()
        .Count();
                    int emptyFields = totalFields - bookedFields;

                    // 5. Chờ thanh toán
                    int pendingPayments = db.LICH_DAT_SAN
                        .Count(x => x.TinhTrang.HasValue && x.TinhTrang.Value == false);

                    // Gán dữ liệu ra UI
                    txtToday.Text = $"{todayBookings} đặt sân";
                    txtTotalCustomers.Text = $"{totalCustomers} khách hàng";
                    txtRevenue.Text = $"{revenue:N0} VND";
                    txtEmptyFields.Text = $"{emptyFields} sân";
                    txtPendingPayments.Text = $"{pendingPayments} đặt sân";
                }
            }
            catch (Exception ex)
            {
                // Nếu có lỗi database, hiển thị dữ liệu mặc định
                txtToday.Text = "0 đặt sân";
                txtTotalCustomers.Text = "0 khách hàng";
                txtRevenue.Text = "0 VND";
                txtEmptyFields.Text = "0 sân";
                txtPendingPayments.Text = "0 đặt sân";
                
                // Có thể log lỗi hoặc hiển thị thông báo
                System.Diagnostics.Debug.WriteLine($"Lỗi khi load dashboard data: {ex.Message}");
            }
        }

        private void btnDangNhap_Click(object sender, RoutedEventArgs e)
        {
            var login = new dangnhap();
            login.Show();
            Close();
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
                    CurrentUser.User = null;
                    
                    // Cập nhật UI
                    UpdateLoginUI();
                    
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

            // Kiểm tra quyền truy cập
            string userRole = CurrentUser.User.VaiTro;

            if (button.Name == "btnSanBong")
            {
                var qlsb = new QuanLySanBong();
                qlsb.Show();
                return;
            }

            if (button.Name == "btnNguoiDung")
            {
                // Chỉ Admin mới được truy cập quản lý người dùng
                if (userRole == "Admin")
                {
                    var qlnd = new QuanLiNguoiDung();
                    qlnd.Show();
                }
                else
                {
                    MessageBox.Show("Bạn không có quyền truy cập chức năng này!", "Thông báo", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                return;
            }

            if (button.Name == "btnThanhToan")
            {
                var qltt = new QuanLyThanhToan();
                qltt.Show();
                return;
            }

            if (button.Name == "btnKhachHang")
            {
                var qlkh = new QuanLiKhachHang();
                qlkh.Show();
                return;
            }

            if (button.Name == "btnLichDatSan")
            {
                var datSanWindow = new DatSanWindow();
                datSanWindow.Show();   // mở cửa sổ mới
                this.Close();          // đóng MainWindow nếu bạn muốn
                return;
            }

            if (button.Name == "btnThietBi")
            {
                var qltb = new QuanLyThietBiVaBaoTri();
                qltb.Show();
                return;
            }

            MessageBox.Show("Chức năng đang phát triển");
        }
    }
}
