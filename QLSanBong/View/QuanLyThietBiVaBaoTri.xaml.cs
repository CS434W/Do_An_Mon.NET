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
    /// Interaction logic for QuanLyThietBiVaBaoTri.xaml
    /// </summary>
    public partial class QuanLyThietBiVaBaoTri : Window
    {
        public QuanLyThietBiVaBaoTri()
        {
            InitializeComponent();
            Loaded += Window_Loaded; // Đăng ký sự kiện Loaded
        }
        ViewModel.QuanLyThietBiVaBaoTriViewModel thietbibaotri = new ViewModel.QuanLyThietBiVaBaoTriViewModel();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            thietbibaotri.LoadThietBi(DG_TB);
            thietbibaotri.LoadBaoTri(DG_BT);
            thietbibaotri.LoadMaSan(cbMaSan);
            thietbibaotri.LoadMaThietBi(cbMaThietBi);
            thietbibaotri.LoadNhanVien(cbNhanVienBT);

        }




        private void Button_Them(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu
                if (string.IsNullOrEmpty(txtMaThietBi.Text) || string.IsNullOrEmpty(txtTenThietBi.Text))
                {
                    MessageBox.Show("Mã thiết bị và tên thiết bị không được để trống");
                    return;
                }

                if (cbMaSan.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn mã sân");
                    return;
                }

                Model.THIET_BI them = new Model.THIET_BI();
                them.MaThietBi = txtMaThietBi.Text;
                them.TenThietBi = txtTenThietBi.Text;
                them.SoLuong = int.Parse(txtSoLuong.Text);
                them.Loai = txtLoai.Text;
                them.NgayNhap = DateTime.Parse(dpNgayNhap.Text);
                them.TinhTrang = cbTinhTrang.SelectedIndex == 1;
                them.MaSan = cbMaSan.SelectedValue?.ToString(); // SỬA: dùng ComboBox

                thietbibaotri.ThemThietBi(them);
                thietbibaotri.LoadThietBi(DG_TB);
                MessageBox.Show("Thêm thiết bị thành công");

                // Clear form sau khi thêm
                ClearFormThietBi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void Button_Sua(object sender, RoutedEventArgs e)
        {
            try
            {
                Model.THIET_BI sua = DG_TB.SelectedItem as Model.THIET_BI;
                if (sua == null)
                {
                    MessageBox.Show("Hãy chọn thiết bị cần sửa");
                    return;
                }

                Model.THIET_BI sua1 = new Model.THIET_BI()
                {
                    MaThietBi = txtMaThietBi.Text,
                    TenThietBi = txtTenThietBi.Text,
                    SoLuong = int.Parse(txtSoLuong.Text),
                    Loai = txtLoai.Text,
                    NgayNhap = DateTime.Parse(dpNgayNhap.Text),
                    TinhTrang = cbTinhTrang.SelectedIndex == 1,
                    MaSan = cbMaSan.SelectedValue?.ToString() // SỬA: dùng ComboBox
                };
                thietbibaotri.SuaThietBi(sua1);
                thietbibaotri.LoadThietBi(DG_TB);
                MessageBox.Show("Sửa thiết bị thành công");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void Button_Xoa(object sender, RoutedEventArgs e)
        {
            try
            {
                Model.THIET_BI xoa = DG_TB.SelectedItem as Model.THIET_BI;
                if (xoa == null)
                {
                    MessageBox.Show("Hãy chọn thiết bị cần xóa");
                    return;
                }

                // Xác nhận trước khi xóa
                var result = MessageBox.Show($"Bạn có chắc muốn xóa thiết bị {xoa.TenThietBi}?",
                                           "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    thietbibaotri.XoaThietBi(xoa);
                    thietbibaotri.LoadThietBi(DG_TB);
                    MessageBox.Show("Xóa thiết bị thành công");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void Button_ThemBT(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaBaoTri.Text))
                {
                    MessageBox.Show("Mã bảo trì không được để trống");
                    return;
                }

                if (cbMaThietBi.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn mã thiết bị");
                    return;
                }

                if (cbNhanVienBT.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn nhân viên BT");
                    return;
                }

                Model.BAO_TRI them = new Model.BAO_TRI();
                them.MaBaoTri = txtMaBaoTri.Text;
                them.MaThietBi = cbMaThietBi.SelectedValue.ToString();
                them.NgayBatDau = DateTime.Parse(dpNgayBatDau.Text);
                them.NgayKetThuc = DateTime.Parse(dpNgayKetThuc.Text);
                them.LoaiBaoTri = txtLoaiBaoTri.Text;
                them.ChiPhi = decimal.Parse(txtChiPhi.Text);
                them.TrangThai = cbTrangThai.SelectedIndex == 1;
                them.NhanVienBT = cbNhanVienBT.SelectedValue.ToString(); // SỬA: dùng SelectedValue

                thietbibaotri.ThemBaoTri(them);
                thietbibaotri.LoadBaoTri(DG_BT);
                MessageBox.Show("Thêm bảo trì thành công");

                // Clear form sau khi thêm
                ClearFormBaoTri();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void Button_SuaBT(object sender, RoutedEventArgs e)
        {
            try
            {
                Model.BAO_TRI sua = DG_BT.SelectedItem as Model.BAO_TRI;
                if (sua == null)
                {
                    MessageBox.Show("Hãy chọn bảo trì cần sửa");
                    return;
                }
                Model.BAO_TRI sua1 = new Model.BAO_TRI()
                {
                    MaBaoTri = txtMaBaoTri.Text,
                    MaThietBi = cbMaThietBi.SelectedValue?.ToString(),
                    NgayBatDau = DateTime.Parse(dpNgayBatDau.Text),
                    NgayKetThuc = DateTime.Parse(dpNgayKetThuc.Text),
                    LoaiBaoTri = txtLoaiBaoTri.Text,
                    ChiPhi = decimal.Parse(txtChiPhi.Text),
                    TrangThai = cbTrangThai.SelectedIndex == 1,
                    NhanVienBT = cbNhanVienBT.SelectedValue?.ToString() // SỬA: dùng SelectedValue
                };
                thietbibaotri.SuaBaoTri(sua1);
                thietbibaotri.LoadBaoTri(DG_BT);
                MessageBox.Show("Sửa bảo trì thành công");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void Button_XoaBT(object sender, RoutedEventArgs e)
        {
            try
            {
                Model.BAO_TRI xoa = DG_BT.SelectedItem as Model.BAO_TRI;
                if (xoa == null)
                {
                    MessageBox.Show("Hãy chọn bảo trì cần xóa");
                    return;
                }

                // Xác nhận trước khi xóa
                var result = MessageBox.Show($"Bạn có chắc muốn xóa bảo trì {xoa.MaBaoTri}?",
                                           "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    thietbibaotri.XoaBaoTri(xoa);
                    thietbibaotri.LoadBaoTri(DG_BT);
                    MessageBox.Show("Xóa bảo trì thành công");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        // Thêm phương thức clear form
        private void ClearFormThietBi()
        {
            txtMaThietBi.Clear();
            txtTenThietBi.Clear();
            txtSoLuong.Clear();
            txtLoai.Clear();
            dpNgayNhap.SelectedDate = null;
            cbTinhTrang.SelectedIndex = 0;
            cbMaSan.SelectedIndex = -1;
        }

        private void ClearFormBaoTri()
        {
            txtMaBaoTri.Clear();
            cbMaThietBi.SelectedIndex = -1;
            dpNgayBatDau.SelectedDate = null;
            dpNgayKetThuc.SelectedDate = null;
            txtLoaiBaoTri.Clear();
            txtChiPhi.Clear();
            cbTrangThai.SelectedIndex = 0;
            cbNhanVienBT.SelectedIndex = -1;
        }
        private void Button_QuayVe(object sender, RoutedEventArgs e)
        {
            // Quay về MainWindow
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }




        private void DG_TB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (DG_TB.SelectedItem == null) return;

                Model.THIET_BI sv = DG_TB.SelectedItem as Model.THIET_BI;
                if (sv == null) return;

                // Hiển thị dữ liệu lên form
                txtMaThietBi.Text = sv.MaThietBi;
                txtTenThietBi.Text = sv.TenThietBi;
                txtSoLuong.Text = sv.SoLuong.ToString();
                txtLoai.Text = sv.Loai;

                // Xử lý ngày nhập
                if (sv.NgayNhap.HasValue)
                    dpNgayNhap.SelectedDate = sv.NgayNhap.Value;
                else
                    dpNgayNhap.SelectedDate = null;

                // Xử lý tình trạng
                cbTinhTrang.SelectedIndex = sv.TinhTrang == true ? 1 : 0;

                // Xử lý mã sân
                if (!string.IsNullOrEmpty(sv.MaSan))
                {
                    foreach (var item in cbMaSan.Items)
                    {
                        var san = item as Model.SAN_BONG;
                        if (san != null && san.MaSan == sv.MaSan)
                        {
                            cbMaSan.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn thiết bị: " + ex.Message);
            }
        }

        private void DG_BT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.BAO_TRI sv = DG_BT.SelectedItem as Model.BAO_TRI;
            if (sv == null) return;

            txtMaBaoTri.Text = sv.MaBaoTri;

            // Chọn mã thiết bị
            if (!string.IsNullOrEmpty(sv.MaThietBi))
            {
                cbMaThietBi.SelectedValue = sv.MaThietBi;
            }

            dpNgayBatDau.Text = sv.NgayBatDau.ToString();
            dpNgayKetThuc.Text = sv.NgayKetThuc.ToString();
            txtLoaiBaoTri.Text = sv.LoaiBaoTri;
            txtChiPhi.Text = sv.ChiPhi.ToString();
            cbTrangThai.SelectedIndex = sv.TrangThai == true ? 1 : 0;

            // Chọn mã nhân viên
            if (!string.IsNullOrEmpty(sv.NhanVienBT))
            {
                cbNhanVienBT.SelectedValue = sv.NhanVienBT;
            }
        }
    }
}
