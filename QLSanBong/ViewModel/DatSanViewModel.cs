using QLSanBong.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QLSanBong.ViewModel
{
    public class DatSanViewModel
    {
        private Entities1 context = new Entities1();

        // Các phương thức load dữ liệu
        public void LoadDanhSachSan(ComboBox cb)
        {
            try
            {
                var danhSachSan = context.SAN_BONG
                    .Where(s => s.TrangThai == true)
                    .ToList();
                cb.ItemsSource = danhSachSan;
                cb.DisplayMemberPath = "TenSan";
                cb.SelectedValuePath = "MaSan";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách sân: " + ex.Message);
            }
        }

        public void LoadDanhSachNhanVien(ComboBox cb)
        {
            try
            {
                var danhSachNhanVien = context.NHAN_VIEN.ToList();
                cb.ItemsSource = danhSachNhanVien;
                cb.DisplayMemberPath = "TenNV";
                cb.SelectedValuePath = "MaNV";

                if (danhSachNhanVien.Count > 0)
                    cb.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách nhân viên: " + ex.Message);
            }
        }

        public void LoadDanhSachDatSan(DataGrid dg)
        {
            try
            {
                // Sửa lỗi: Không dùng .Date trong LINQ to Entities
                var danhSach = from l in context.LICH_DAT_SAN
                               join k in context.KHACH_HANG on l.MaKH equals k.MaKH
                               join s in context.SAN_BONG on l.MaSan equals s.MaSan
                               join n in context.NHAN_VIEN on l.MaNV equals n.MaNV into nv
                               from n in nv.DefaultIfEmpty()
                               select new
                               {
                                   l.MaDatSan,
                                   k.TenKH,
                                   k.SDT,
                                   s.TenSan,
                                   ThoiGianBatDau = l.ThoiGianBatDau, // Giữ nguyên DateTime
                                   ThoiGianKetThuc = l.ThoiGianKetThuc,
                                   l.TinhTrang,
                                   TenNV = n != null ? n.TenNV : "Không xác định"
                               };

                // Chuyển sang list trước rồi mới xử lý Date
                var result = danhSach.ToList().Select(x => new
                {
                    x.MaDatSan,
                    x.TenKH,
                    x.SDT,
                    x.TenSan,
                    NgayDat = x.ThoiGianBatDau.HasValue ? x.ThoiGianBatDau.Value.Date : DateTime.MinValue,
                    GioBatDau = x.ThoiGianBatDau.HasValue ? x.ThoiGianBatDau.Value.ToString("HH:mm") : "",
                    GioKetThuc = x.ThoiGianKetThuc.HasValue ? x.ThoiGianKetThuc.Value.ToString("HH:mm") : "",
                    TinhTrangText = x.TinhTrang == true ? "Đang hoạt động" : "Đã hủy",
                    x.TenNV
                });

                dg.ItemsSource = result.ToList();

                // Debug: Kiểm tra số lượng bản ghi
                MessageBox.Show($"Đã tải {result.Count()} bản ghi đặt sân");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách đặt sân: {ex.Message}\n{ex.InnerException?.Message}");
            }
        }

        public void HienThiLichSan(DataGrid dg, DateTime ngay)
        {
            try
            {
                var ds = from l in context.LICH_DAT_SAN
                         join s in context.SAN_BONG on l.MaSan equals s.MaSan
                         where l.ThoiGianBatDau.HasValue && l.TinhTrang == true
                         select new
                         {
                             l.ThoiGianBatDau,
                             l.ThoiGianKetThuc,
                             s.TenSan,
                             s.LoaiSan,
                             s.DonGia,
                             TinhTrang = "Đã đặt"
                         };

                var result = ds.ToList()
                    .Where(x => x.ThoiGianBatDau.Value.Date == ngay.Date)
                    .Select(x => new
                    {
                        ThoiGianBatDau = x.ThoiGianBatDau.Value.ToString("HH:mm"),
                        ThoiGianKetThuc = x.ThoiGianKetThuc.Value.ToString("HH:mm"),
                        x.TenSan,
                        x.LoaiSan,
                        x.DonGia,
                        x.TinhTrang
                    });

                dg.ItemsSource = result.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hiển thị lịch sân: " + ex.Message);
            }
        }

        // Tạo mã mới
        public string TaoMaDatSanMoi()
        {
            try
            {
                int count = context.LICH_DAT_SAN.Count() + 1;
                return "DS" + count.ToString("D4");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo mã đặt sân: " + ex.Message);
                return "DS0000";
            }
        }

        public string TaoMaKHMoi()
        {
            try
            {
                int count = context.KHACH_HANG.Count() + 1;
                return "KH" + count.ToString("D4");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo mã khách hàng: " + ex.Message);
                return "KH0000";
            }
        }

        // Kiểm tra sân trống
        public bool KiemTraSanTrong(string maSan, DateTime thoiGianBatDau, DateTime thoiGianKetThuc)
        {
            try
            {
                var datSanTrung = context.LICH_DAT_SAN
                    .Where(lds => lds.MaSan == maSan
                           && lds.TinhTrang == true
                           && ((lds.ThoiGianBatDau >= thoiGianBatDau && lds.ThoiGianBatDau < thoiGianKetThuc)
                           || (lds.ThoiGianKetThuc > thoiGianBatDau && lds.ThoiGianKetThuc <= thoiGianKetThuc)
                           || (lds.ThoiGianBatDau <= thoiGianBatDau && lds.ThoiGianKetThuc >= thoiGianKetThuc)))
                    .FirstOrDefault();

                return datSanTrung == null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kiểm tra sân trống: " + ex.Message);
                return false;
            }
        }

        // Tính thành tiền
        public decimal TinhThanhTien(SAN_BONG san, string gioBatDau, string gioKetThuc)
        {
            try
            {
                TimeSpan tsBatDau = TimeSpan.Parse(gioBatDau);
                TimeSpan tsKetThuc = TimeSpan.Parse(gioKetThuc);

                double soGio = (tsKetThuc - tsBatDau).TotalHours;

                if (soGio <= 0) return 0;

                return (san.DonGia ?? 0) * (decimal)soGio;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tính tiền: " + ex.Message);
                return 0;
            }
        }

        // Đặt sân
        public void DatSan(string maDatSan, string maSan, string tenKH, string sdt, string ghiChuKH,
                          DateTime ngayDat, string gioBatDau, string gioKetThuc, string maNV)
        {
            try
            {
                // Kiểm tra sân trống
                DateTime thoiGianBatDau = ngayDat.Date + TimeSpan.Parse(gioBatDau);
                DateTime thoiGianKetThuc = ngayDat.Date + TimeSpan.Parse(gioKetThuc);

                if (!KiemTraSanTrong(maSan, thoiGianBatDau, thoiGianKetThuc))
                {
                    MessageBox.Show("Sân đã được đặt trong khoảng thời gian này! Vui lòng kiểm tra lại.");
                    return;
                }

                // Lưu thông tin khách hàng
                string maKH = LuuThongTinKhachHang(tenKH, sdt, ghiChuKH);

                // Lưu thông tin đặt sân
                var datSan = new LICH_DAT_SAN
                {
                    MaDatSan = maDatSan,
                    MaSan = maSan,
                    MaKH = maKH,
                    ThoiGianBatDau = thoiGianBatDau,
                    ThoiGianKetThuc = thoiGianKetThuc,
                    TinhTrang = true,
                    MaNV = maNV
                };

                context.LICH_DAT_SAN.Add(datSan);
                context.SaveChanges();

                MessageBox.Show("Đặt sân thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đặt sân: " + ex.Message);
            }
        }

        private string LuuThongTinKhachHang(string tenKH, string sdt, string ghiChu)
        {
            var existingKH = context.KHACH_HANG.FirstOrDefault(kh => kh.SDT == sdt);

            if (existingKH != null)
            {
                existingKH.TenKH = tenKH;
                existingKH.GhiChu = ghiChu;
                context.SaveChanges();
                return existingKH.MaKH;
            }
            else
            {
                string maKHMoi = TaoMaKHMoi();
                var newKH = new KHACH_HANG
                {
                    MaKH = maKHMoi,
                    TenKH = tenKH,
                    SDT = sdt,
                    GhiChu = ghiChu
                };
                context.KHACH_HANG.Add(newKH);
                context.SaveChanges();
                return maKHMoi;
            }
        }

        // Hủy đặt sân
        public void HuyDatSan(string maDatSan)
        {
            try
            {
                var datSan = context.LICH_DAT_SAN.Find(maDatSan);
                if (datSan != null)
                {
                    datSan.TinhTrang = false;
                    context.SaveChanges();
                    MessageBox.Show("Hủy đặt sân thành công!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hủy đặt sân: " + ex.Message);
            }
        }

        // Tìm kiếm đặt sân
        public void TimKiemDatSan(DataGrid dg, string timKiem, int loaiTimKiem)
        {
            try
            {
                var danhSach = from l in context.LICH_DAT_SAN
                               join k in context.KHACH_HANG on l.MaKH equals k.MaKH
                               join s in context.SAN_BONG on l.MaSan equals s.MaSan
                               join n in context.NHAN_VIEN on l.MaNV equals n.MaNV into nv
                               from n in nv.DefaultIfEmpty()
                               select new
                               {
                                   l.MaDatSan,
                                   k.TenKH,
                                   k.SDT,
                                   s.TenSan,
                                   ThoiGianBatDau = l.ThoiGianBatDau,
                                   l.TinhTrang,
                                   TenNV = n != null ? n.TenNV : "Không xác định"
                               };

                // Chuyển sang list trước
                var list = danhSach.ToList().Select(x => new
                {
                    x.MaDatSan,
                    x.TenKH,
                    x.SDT,
                    x.TenSan,
                    NgayDat = x.ThoiGianBatDau.HasValue ? x.ThoiGianBatDau.Value.Date : DateTime.MinValue,
                    GioBatDau = x.ThoiGianBatDau.HasValue ? x.ThoiGianBatDau.Value.ToString("HH:mm") : "",
                    GioKetThuc = x.ThoiGianBatDau.HasValue ? x.ThoiGianBatDau.Value.AddHours(1).ToString("HH:mm") : "", // Giả sử mỗi đặt là 1 giờ
                    TinhTrangText = x.TinhTrang == true ? "Đang hoạt động" : "Đã hủy",
                    x.TenNV
                });

                if (!string.IsNullOrEmpty(timKiem))
                {
                    switch (loaiTimKiem)
                    {
                        case 0: // Theo tên KH
                            list = list.Where(x => x.TenKH.ToLower().Contains(timKiem.ToLower()));
                            break;
                        case 1: // Theo sân
                            list = list.Where(x => x.TenSan.ToLower().Contains(timKiem.ToLower()));
                            break;
                        case 2: // Theo ngày
                            list = list.Where(x => x.NgayDat.ToString("dd/MM/yyyy").Contains(timKiem));
                            break;
                    }
                }

                dg.ItemsSource = list.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message);
            }
        }

        // Thống kê
        public void ThongKeDatSan(TextBlock tbTongSo, TextBlock tbDangHoatDong, TextBlock tbDaHuy)
        {
            try
            {
                int tongSo = context.LICH_DAT_SAN.Count();
                int dangHoatDong = context.LICH_DAT_SAN.Count(x => x.TinhTrang == true);
                int daHuy = context.LICH_DAT_SAN.Count(x => x.TinhTrang == false);

                tbTongSo.Text = tongSo.ToString();
                tbDangHoatDong.Text = dangHoatDong.ToString();
                tbDaHuy.Text = daHuy.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thống kê: " + ex.Message);
            }
        }
    }
}