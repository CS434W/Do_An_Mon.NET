using System;
using System.Windows;
using System.Windows.Controls;

namespace QLSanBong.View
{
    public partial class QuanLyThanhToan : Window
    {
        public QuanLyThanhToan()
        {
            InitializeComponent();
        }

        ViewModel.QuanLyThanhToanViewModel ttvm = new ViewModel.QuanLyThanhToanViewModel();

        private void BtnQuayVe_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ttvm.LoadThanhToan(dg_DanhSachThanhToan);
                ttvm.LoadDanhSachDatSan(cb_MaDat);
                ttvm.LoadNhanVien(cb_NhanVienThanhToan);
                ttvm.LoadCongNo(dg_CongNo, "");
                ttvm.LoadBaoCao(dg_BaoCao, dp_TuNgay.SelectedDate, dp_DenNgay.SelectedDate, lbl_TongDoanhThu, lbl_TongCongNo);

                dp_NgayTT.SelectedDate = DateTime.Now;
                dp_TuNgay.SelectedDate = DateTime.Now.AddDays(-30);
                dp_DenNgay.SelectedDate = DateTime.Now;

                txt_MaTT.Text = "TT" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        private void btn_Luu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txt_MaTT.Text))
                {
                    MessageBox.Show("Mã thanh toán không được để trống");
                    return;
                }

                if (cb_MaDat.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn mã đặt sân");
                    return;
                }

                if (string.IsNullOrEmpty(txt_SoTien.Text) || !decimal.TryParse(txt_SoTien.Text, out decimal soTien) || soTien <= 0)
                {
                    MessageBox.Show("Số tiền không hợp lệ");
                    return;
                }

                if (cb_NhanVienThanhToan.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn nhân viên thanh toán");
                    return;
                }

                if (dp_NgayTT.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày thanh toán");
                    return;
                }

                Model.THANH_TOAN tt = new Model.THANH_TOAN
                {
                    MaTT = txt_MaTT.Text,
                    MaDat = cb_MaDat.SelectedValue?.ToString(),
                    SoTien = soTien,
                    NgayTT = dp_NgayTT.SelectedDate,
                    GhiChu = txt_GhiChu.Text,
                    MaNV = cb_NhanVienThanhToan.SelectedValue?.ToString()
                };

                var result = ttvm.ThemThanhToan(tt, cb_MaDat.SelectedValue?.ToString());

                if (result)
                {
                    ttvm.LoadThanhToan(dg_DanhSachThanhToan);
                    ttvm.LoadDanhSachDatSan(cb_MaDat);
                    ttvm.LoadCongNo(dg_CongNo, "");
                    ttvm.LoadBaoCao(dg_BaoCao, dp_TuNgay.SelectedDate, dp_DenNgay.SelectedDate, lbl_TongDoanhThu, lbl_TongCongNo);

                    MessageBox.Show("Thêm thanh toán thành công");
                    btn_TaoMoi_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Lỗi khi thêm thanh toán");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btn_Xoa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = dg_DanhSachThanhToan.SelectedItem;
                if (selectedItem == null)
                {
                    MessageBox.Show("Hãy chọn thanh toán cần xoá");
                    return;
                }

                dynamic item = selectedItem;
                string maTT = item.MaTT;

                var result = MessageBox.Show("Bạn có chắc chắn muốn xoá thanh toán này?", "Xác nhận xoá",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Model.THANH_TOAN ttXoa = new Model.THANH_TOAN { MaTT = maTT };
                    ttvm.XoaThanhToan(ttXoa);

                    ttvm.LoadThanhToan(dg_DanhSachThanhToan);
                    ttvm.LoadDanhSachDatSan(cb_MaDat);
                    ttvm.LoadCongNo(dg_CongNo, "");
                    MessageBox.Show("Xoá thanh toán thành công");
                    btn_TaoMoi_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xoá: " + ex.Message);
            }
        }

        private void btn_TaoMoi_Click(object sender, RoutedEventArgs e)
        {
            txt_MaTT.Text = "TT" + DateTime.Now.ToString("yyyyMMddHHmmss");
            cb_MaDat.SelectedIndex = -1;
            txt_TenKH.Text = "";
            txt_San.Text = "";
            txt_ThoiGianBatDau.Text = "";
            txt_ThoiGianKetThuc.Text = "";
            txt_SoGioThue.Text = "";
            txt_DonGia.Text = "";
            txt_ThanhTien.Text = "";
            txt_SoTien.Text = "";
            txt_GhiChu.Text = "";
            cb_NhanVienThanhToan.SelectedIndex = -1;
            dp_NgayTT.SelectedDate = DateTime.Now;
        }

        private void cb_MaDat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cb_MaDat.SelectedItem != null)
                {
                    dynamic selectedItem = cb_MaDat.SelectedItem;
                    string maDat = selectedItem.MaDatSan;

                    var thongTin = ttvm.LayThongTinDatSan(maDat);
                    if (thongTin != null)
                    {
                        txt_TenKH.Text = thongTin.TenKH ?? "";
                        txt_San.Text = thongTin.TenSan ?? "";
                        txt_ThoiGianBatDau.Text = thongTin.ThoiGianBatDau?.ToString("dd/MM/yyyy HH:mm") ?? "";
                        txt_ThoiGianKetThuc.Text = thongTin.ThoiGianKetThuc?.ToString("dd/MM/yyyy HH:mm") ?? "";
                        txt_SoGioThue.Text = thongTin.SoGioThue.ToString("F1");
                        txt_DonGia.Text = thongTin.DonGia?.ToString("N0") ?? "0";
                        txt_ThanhTien.Text = thongTin.ThanhTien.ToString("N0");
                        txt_SoTien.Text = thongTin.ThanhTien.ToString("N0");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông tin đặt sân: " + ex.Message);
            }
        }

        private void dg_DanhSachThanhToan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = dg_DanhSachThanhToan.SelectedItem;
                if (selectedItem == null) return;

                dynamic item = selectedItem;
                txt_MaTT.Text = item.MaTT;
                cb_MaDat.SelectedValue = item.MaDat;
                txt_SoTien.Text = item.SoTien?.ToString("N0");
                dp_NgayTT.SelectedDate = item.NgayTT;
                txt_GhiChu.Text = item.GhiChu;
                cb_NhanVienThanhToan.SelectedValue = item.MaNV;

                var thongTin = ttvm.LayThongTinDatSan(item.MaDat);
                if (thongTin != null)
                {
                    txt_TenKH.Text = thongTin.TenKH ?? "";
                    txt_San.Text = thongTin.TenSan ?? "";
                    txt_ThoiGianBatDau.Text = thongTin.ThoiGianBatDau?.ToString("dd/MM/yyyy HH:mm") ?? "";
                    txt_ThoiGianKetThuc.Text = thongTin.ThoiGianKetThuc?.ToString("dd/MM/yyyy HH:mm") ?? "";
                    txt_SoGioThue.Text = thongTin.SoGioThue.ToString("F1");
                    txt_DonGia.Text = thongTin.DonGia?.ToString("N0") ?? "0";
                    txt_ThanhTien.Text = thongTin.ThanhTien.ToString("N0");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông tin: " + ex.Message);
            }
        }

        private void dg_CongNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Có thể thêm xử lý khi chọn row trong công nợ nếu cần
        }

        private void btn_TimKiemCongNo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ttvm.LoadCongNo(dg_CongNo, txt_TimKiemKH.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message);
            }
        }

        private void btn_LamMoiCongNo_Click(object sender, RoutedEventArgs e)
        {
            txt_TimKiemKH.Text = "";
            ttvm.LoadCongNo(dg_CongNo, "");
        }

        private void btn_ThanhToanCongNo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dg_CongNo.SelectedItem;
                if (selectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng cần thanh toán công nợ");
                    return;
                }

                string maKH = selectedItem.MaKH;
                string tenKH = selectedItem.TenKH;

                var result = MessageBox.Show($"Bạn có muốn thanh toán công nợ cho khách hàng: {tenKH}?",
                    "Thanh toán công nợ", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    TabControl.SelectedIndex = 0;
                    ttvm.LoadDanhSachDatSanTheoKH(cb_MaDat, maKH);
                    MessageBox.Show("Vui lòng chọn các đặt sân cần thanh toán trong danh sách");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btn_XemBaoCao_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dp_TuNgay.SelectedDate > dp_DenNgay.SelectedDate)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc");
                    return;
                }

                ttvm.LoadBaoCao(dg_BaoCao, dp_TuNgay.SelectedDate, dp_DenNgay.SelectedDate, lbl_TongDoanhThu, lbl_TongCongNo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo: " + ex.Message);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (TabControl.SelectedIndex == 1)
                {
                    ttvm.LoadCongNo(dg_CongNo, txt_TimKiemKH.Text);
                }
                else if (TabControl.SelectedIndex == 2)
                {
                    ttvm.LoadBaoCao(dg_BaoCao, dp_TuNgay.SelectedDate, dp_DenNgay.SelectedDate, lbl_TongDoanhThu, lbl_TongCongNo);
                }
            }
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
