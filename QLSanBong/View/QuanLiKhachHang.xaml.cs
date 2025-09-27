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

namespace QLSanBong.View
{
    /// <summary>
    /// Interaction logic for QuanLiKhachHang.xaml
    /// </summary>
    public partial class QuanLiKhachHang : Window
    {
        ViewModel.QLyKhachHangViewModel khvm = new ViewModel.QLyKhachHangViewModel();
        public QuanLiKhachHang()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load data từ database vào grid khi window load
            khvm.LoadKHACHHANG(dgvKhachHang);
        }
        //btn_Thêm
        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenKH.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSDT.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại");
                return;
            }

            // Tạo mã KH tự động
            string newMaKH = "KH" + DateTime.Now.ToString("yyyyMMddHHmmss");

            Model.KHACH_HANG kh = new Model.KHACH_HANG
            {
                MaKH = newMaKH,
                TenKH = txtTenKH.Text,
                SDT = txtSDT.Text,
                GhiChu = txtGhiChu.Text
            };

            try
            {
                khvm.ThemKHACHHANG(kh);
                khvm.LoadKHACHHANG(dgvKhachHang);
                MessageBox.Show("Thêm khách hàng thành công");
                LamMoiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm khách hàng: " + ex.Message);
            }
        }
        //btn_sửa
        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            Model.KHACH_HANG khchon = dgvKhachHang.SelectedItem as Model.KHACH_HANG;
            if (khchon == null)
            {
                MessageBox.Show("Hãy chọn khách hàng cần sửa");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenKH.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSDT.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại");
                return;
            }

            Model.KHACH_HANG kh = new Model.KHACH_HANG
            {
                MaKH = txtMaKH.Text,
                TenKH = txtTenKH.Text,
                SDT = txtSDT.Text,
                GhiChu = txtGhiChu.Text
            };

            try
            {
                khvm.SuaKhachHang(khchon.MaKH, kh);
                khvm.LoadKHACHHANG(dgvKhachHang);
                MessageBox.Show("Sửa khách hàng thành công");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa khách hàng: " + ex.Message);
            }
        }

        //btn_xóa
        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            Model.KHACH_HANG khchon = dgvKhachHang.SelectedItem as Model.KHACH_HANG;
            if (khchon == null)
            {
                MessageBox.Show("Hãy chọn khách hàng cần xoá");
                return;
            }

            try
            {
                khvm.XoaKhachHang(khchon);
                khvm.LoadKHACHHANG(dgvKhachHang);
                MessageBox.Show("Xoá khách hàng thành công");
                LamMoiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa khách hàng: " + ex.Message);
            }
        }

        private void btnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            khvm.TimKiemKhachHang(dgvKhachHang, txtTimKiem.Text);
        }

        private void dgvKhachHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.KHACH_HANG kh = dgvKhachHang.SelectedItem as Model.KHACH_HANG;
            if (kh == null) return;

            txtMaKH.Text = kh.MaKH;
            txtTenKH.Text = kh.TenKH;
            txtSDT.Text = kh.SDT;
            txtGhiChu.Text = kh.GhiChu;

            // Load lịch sử đặt sân của khách hàng được chọn
            khvm.LoadLichSuDatSan(dgvLichSuDatSan, kh.MaKH);
        }

        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            khvm.LoadKHACHHANG(dgvKhachHang);
            LamMoiForm();
            txtTimKiem.Text = "";
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
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
        private void LamMoiForm()
        {
            txtMaKH.Text = "";
            txtTenKH.Text = "";
            txtSDT.Text = "";
            txtGhiChu.Text = "";
            dgvLichSuDatSan.ItemsSource = null;
        }
        private void DgvKhachHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.KHACH_HANG kh = dgvKhachHang.SelectedItem as Model.KHACH_HANG;
            if (kh == null)
            {
                // Nếu không chọn khách hàng nào, clear lịch sử
                dgvLichSuDatSan.ItemsSource = null;
                return;
            }

            // Hiển thị thông tin khách hàng được chọn
            txtMaKH.Text = kh.MaKH;
            txtTenKH.Text = kh.TenKH;
            txtSDT.Text = kh.SDT;
            txtGhiChu.Text = kh.GhiChu;

            // QUAN TRỌNG: Load lịch sử đặt sân của khách hàng được chọn
            khvm.LoadLichSuDatSan(dgvLichSuDatSan, kh.MaKH);
        }



    }
}
