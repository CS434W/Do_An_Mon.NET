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
        Model.Entities1 db = new Model.Entities1();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            LoadDashboardData();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Ẩn nút đăng nhập nếu đã có người dùng đăng nhập
            if (CurrentUser.User != null)
            {
                btnDangNhap.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadDashboardData()
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
                txtEmptyFields.Text = $"{emptyFields}/{totalFields} sân";
                txtPendingPayments.Text = $"{pendingPayments} đặt sân";
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

            if (sender == btnKhachHang)
            {
                var khachHangWindow = new QuanLiKhachHang();
                khachHangWindow.Show();  
                this.Close();           
            }
            else if (sender == btnThanhToan)
            {
                var thanhToanWindow = new QuanLyThanhToan();
                thanhToanWindow.Show();
                this.Close();
            }
            else if (sender == btnNguoiDung)
            {
                var nguoiDungWindow = new QuanLiNguoiDung();
                nguoiDungWindow.Show();
                this.Close();
            }


            // Đã đăng nhập: tại đây có thể điều hướng tới trang tương ứng
            // Ví dụ: mở cửa sổ quản lý, v.v. (chưa yêu cầu nên để trống)
        }
    }
}
