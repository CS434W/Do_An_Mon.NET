using QLSanBong.Model;
using QLSanBong.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QLSanBong.View
{
    public partial class DatSanWindow : Window
    {
        DatSanViewModel dsVM = new DatSanViewModel();

        public DatSanWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            // Load dữ liệu
            dsVM.LoadDanhSachSan(cb_San);
            dsVM.LoadDanhSachNhanVien(cb_NhanVien);

            // Tạo mã mới
            txt_MaDatSan.Text = dsVM.TaoMaDatSanMoi();
            txt_MaKH.Text = dsVM.TaoMaKHMoi();

            // Set ngày mặc định
            dp_NgayDat.SelectedDate = DateTime.Today;
            dp_NgayXem.SelectedDate = DateTime.Today;

            // Hiển thị dữ liệu
            dsVM.HienThiLichSan(dg_LichSan, DateTime.Today);
            dsVM.LoadDanhSachDatSan(dg_DanhSachDatSan);
            dsVM.ThongKeDatSan(tb_TongSo, tb_DangHoatDong, tb_DaHuy);
        }

        private void cb_San_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_San.SelectedItem is SAN_BONG san)
            {
                txt_DonGia.Text = san.DonGia?.ToString("N0") ?? "0";
                TinhThanhTien();
            }
        }

        private void cb_GioBatDau_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TinhThanhTien();
        }

        private void cb_GioKetThuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TinhThanhTien();
        }

        private void TinhThanhTien()
        {
            if (cb_San.SelectedItem is SAN_BONG san &&
                cb_GioBatDau.SelectedItem != null &&
                cb_GioKetThuc.SelectedItem != null)
            {
                string gioBatDau = (cb_GioBatDau.SelectedItem as ComboBoxItem).Content.ToString();
                string gioKetThuc = (cb_GioKetThuc.SelectedItem as ComboBoxItem).Content.ToString();

                decimal thanhTien = dsVM.TinhThanhTien(san, gioBatDau, gioKetThuc);
                txt_ThanhTien.Text = thanhTien.ToString("N0");
            }
            else
            {
                txt_ThanhTien.Text = "0";
            }
        }

        private void btn_KiemTraSanTrong_Click(object sender, RoutedEventArgs e)
        {
            if (dp_NgayDat.SelectedDate == null || cb_GioBatDau.SelectedItem == null ||
                cb_GioKetThuc.SelectedItem == null || cb_San.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin: sân, ngày và giờ đặt");
                return;
            }

            DateTime ngayDat = dp_NgayDat.SelectedDate.Value;
            string gioBatDau = (cb_GioBatDau.SelectedItem as ComboBoxItem).Content.ToString();
            string gioKetThuc = (cb_GioKetThuc.SelectedItem as ComboBoxItem).Content.ToString();

            DateTime thoiGianBatDau = ngayDat.Date + TimeSpan.Parse(gioBatDau);
            DateTime thoiGianKetThuc = ngayDat.Date + TimeSpan.Parse(gioKetThuc);

            string maSan = (cb_San.SelectedItem as SAN_BONG).MaSan;
            bool sanTrong = dsVM.KiemTraSanTrong(maSan, thoiGianBatDau, thoiGianKetThuc);

            if (sanTrong)
            {
                tb_ThongBao.Text = "Sân trống! Có thể đặt sân.";
                tb_ThongBao.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                tb_ThongBao.Text = "Sân đã được đặt! Vui lòng chọn sân khác hoặc thời gian khác.";
                tb_ThongBao.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void btn_DatSan_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraDuLieuHopLe()) return;

            string maDatSan = txt_MaDatSan.Text;
            string maSan = (cb_San.SelectedItem as SAN_BONG).MaSan;
            string tenKH = txt_TenKH.Text;
            string sdt = txt_SDT.Text;
            string ghiChuKH = txt_GhiChuKH.Text;
            DateTime ngayDat = dp_NgayDat.SelectedDate.Value;
            string gioBatDau = (cb_GioBatDau.SelectedItem as ComboBoxItem).Content.ToString();
            string gioKetThuc = (cb_GioKetThuc.SelectedItem as ComboBoxItem).Content.ToString();
            string maNV = (cb_NhanVien.SelectedItem as NHAN_VIEN)?.MaNV;

            dsVM.DatSan(maDatSan, maSan, tenKH, sdt, ghiChuKH, ngayDat, gioBatDau, gioKetThuc, maNV);

            // Refresh dữ liệu
            dsVM.LoadDanhSachDatSan(dg_DanhSachDatSan);
            dsVM.HienThiLichSan(dg_LichSan, DateTime.Today);
            dsVM.ThongKeDatSan(tb_TongSo, tb_DangHoatDong, tb_DaHuy);

            btn_TaoMoi_Click(sender, e);
        }

        private bool KiemTraDuLieuHopLe()
        {
            if (string.IsNullOrEmpty(txt_TenKH.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng");
                return false;
            }

            if (string.IsNullOrEmpty(txt_SDT.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại");
                return false;
            }

            if (cb_San.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn sân");
                return false;
            }

            if (dp_NgayDat.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày đặt");
                return false;
            }

            if (cb_GioBatDau.SelectedItem == null || cb_GioKetThuc.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn giờ bắt đầu và kết thúc");
                return false;
            }

            string gioBatDau = (cb_GioBatDau.SelectedItem as ComboBoxItem).Content.ToString();
            string gioKetThuc = (cb_GioKetThuc.SelectedItem as ComboBoxItem).Content.ToString();

            if (TimeSpan.Parse(gioKetThuc) <= TimeSpan.Parse(gioBatDau))
            {
                MessageBox.Show("Giờ kết thúc phải lớn hơn giờ bắt đầu");
                return false;
            }

            return true;
        }

        private void btn_TaoMoi_Click(object sender, RoutedEventArgs e)
        {
            txt_MaDatSan.Text = dsVM.TaoMaDatSanMoi();
            txt_MaKH.Text = dsVM.TaoMaKHMoi();
            txt_TenKH.Clear();
            txt_SDT.Clear();
            txt_GhiChuKH.Clear();
            cb_San.SelectedIndex = -1;
            dp_NgayDat.SelectedDate = DateTime.Today;
            cb_GioBatDau.SelectedIndex = -1;
            cb_GioKetThuc.SelectedIndex = -1;
            txt_DonGia.Clear();
            txt_ThanhTien.Clear();
            tb_ThongBao.Text = "";
        }

        private void btn_HuyDat_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_MaDatSan.Text) || txt_MaDatSan.Text == "DS0000")
            {
                MessageBox.Show("Không có đặt sân nào để hủy");
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn hủy đặt sân này?", "Xác nhận hủy",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                dsVM.HuyDatSan(txt_MaDatSan.Text);

                // Refresh dữ liệu
                dsVM.LoadDanhSachDatSan(dg_DanhSachDatSan);
                dsVM.HienThiLichSan(dg_LichSan, DateTime.Today);
                dsVM.ThongKeDatSan(tb_TongSo, tb_DangHoatDong, tb_DaHuy);
                btn_TaoMoi_Click(sender, e);
            }
        }

        private void btn_HuyDatTrongDanhSach_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dg_DanhSachDatSan.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một đặt sân để hủy");
                return;
            }

            // Lấy mã đặt sân từ selected item
            var maDatSanProperty = selectedItem.GetType().GetProperty("MaDatSan");
            if (maDatSanProperty == null)
            {
                MessageBox.Show("Không thể lấy thông tin đặt sân");
                return;
            }

            string maDatSan = maDatSanProperty.GetValue(selectedItem) as string;

            var result = MessageBox.Show("Bạn có chắc chắn muốn hủy đặt sân này?", "Xác nhận hủy",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                dsVM.HuyDatSan(maDatSan);

                // Refresh dữ liệu
                dsVM.LoadDanhSachDatSan(dg_DanhSachDatSan);
                dsVM.HienThiLichSan(dg_LichSan, DateTime.Today);
                dsVM.ThongKeDatSan(tb_TongSo, tb_DangHoatDong, tb_DaHuy);
            }
        }

        private void btn_XemLich_Click(object sender, RoutedEventArgs e)
        {
            if (dp_NgayXem.SelectedDate.HasValue)
            {
                dsVM.HienThiLichSan(dg_LichSan, dp_NgayXem.SelectedDate.Value);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ngày để xem lịch.");
            }
        }

        private void dp_NgayXem_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dp_NgayXem.SelectedDate.HasValue)
            {
                dsVM.HienThiLichSan(dg_LichSan, dp_NgayXem.SelectedDate.Value);
            }
        }

        private void txt_TimKiem_TextChanged(object sender, TextChangedEventArgs e)
        {
            dsVM.TimKiemDatSan(dg_DanhSachDatSan, txt_TimKiem.Text, cb_LoaiTimKiem.SelectedIndex);
        }

        private void btn_LamMoi_Click(object sender, RoutedEventArgs e)
        {
            txt_TimKiem.Clear();
            dsVM.LoadDanhSachDatSan(dg_DanhSachDatSan);
        }

        private void btn_QuayVe_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dp_NgayDat_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void dg_LichSan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
      
        }

        private void dg_DanhSachDatSan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }



        
    }
}
