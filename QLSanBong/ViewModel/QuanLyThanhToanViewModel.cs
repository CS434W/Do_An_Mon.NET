using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace QLSanBong.ViewModel
{
    internal class QuanLyThanhToanViewModel
    {
        Model.Entities1 db = new Model.Entities1();

        public bool ThemThanhToan(Model.THANH_TOAN tt, string maDat)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (db.THANH_TOAN.Any(x => x.MaTT == tt.MaTT))
                    {
                        throw new Exception("Mã thanh toán đã tồn tại");
                    }

                    db.THANH_TOAN.Add(tt);
                    db.SaveChanges();

                    var datSan = db.LICH_DAT_SAN.Find(maDat);
                    if (datSan != null)
                    {
                        datSan.TinhTrang = true;
                        db.SaveChanges();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void XoaThanhToan(Model.THANH_TOAN ttXoa)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Model.THANH_TOAN tt = db.THANH_TOAN.Find(ttXoa.MaTT);
                    if (tt != null)
                    {
                        var datSan = db.LICH_DAT_SAN.Find(tt.MaDat);
                        if (datSan != null)
                        {
                            datSan.TinhTrang = false;
                        }

                        db.THANH_TOAN.Remove(tt);
                        db.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void LoadThanhToan(DataGrid dg)
        {
            try
            {
                var danhSachThanhToan = db.THANH_TOAN
                    .Include(tt => tt.LICH_DAT_SAN)
                    .Include(tt => tt.LICH_DAT_SAN.KHACH_HANG)
                    .Include(tt => tt.LICH_DAT_SAN.SAN_BONG)
                    .Include(tt => tt.NHAN_VIEN)
                    .ToList()
                    .Select(tt => new
                    {
                        tt.MaTT,
                        tt.MaDat,
                        TenKH = tt.LICH_DAT_SAN?.KHACH_HANG?.TenKH ?? "",
                        TenSan = tt.LICH_DAT_SAN?.SAN_BONG?.TenSan ?? "",
                        tt.SoTien,
                        tt.NgayTT,
                        TenNV = tt.NHAN_VIEN?.TenNV ?? "",
                        tt.GhiChu,
                        MaNV = tt.MaNV
                    }).ToList();

                dg.ItemsSource = danhSachThanhToan;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách thanh toán: " + ex.Message);
                dg.ItemsSource = new List<dynamic>();
            }
        }

        public void LoadDanhSachDatSan(ComboBox cb)
        {
            try
            {
                var danhSachDatSan = db.LICH_DAT_SAN
                    .Where(d => d.TinhTrang == false)
                    .Include(d => d.KHACH_HANG)
                    .Include(d => d.SAN_BONG)
                    .ToList()
                    .Select(d => new
                    {
                        d.MaDatSan,
                        d.KHACH_HANG.TenKH,
                        d.SAN_BONG.TenSan,
                        d.ThoiGianBatDau,
                        d.ThoiGianKetThuc,
                        d.SAN_BONG.DonGia
                    }).ToList();

                cb.ItemsSource = danhSachDatSan;
                cb.DisplayMemberPath = "MaDatSan";
                cb.SelectedValuePath = "MaDatSan";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách đặt sân: " + ex.Message);
                cb.ItemsSource = new List<dynamic>();
            }
        }

        public void LoadDanhSachDatSanTheoKH(ComboBox cb, string maKH)
        {
            try
            {
                var danhSachDatSan = db.LICH_DAT_SAN
                    .Where(d => d.TinhTrang == false && d.MaKH == maKH)
                    .Include(d => d.KHACH_HANG)
                    .Include(d => d.SAN_BONG)
                    .ToList()
                    .Select(d => new
                    {
                        d.MaDatSan,
                        d.KHACH_HANG.TenKH,
                        d.SAN_BONG.TenSan,
                        d.ThoiGianBatDau,
                        d.ThoiGianKetThuc,
                        d.SAN_BONG.DonGia
                    }).ToList();

                cb.ItemsSource = danhSachDatSan;
                cb.DisplayMemberPath = "MaDatSan";
                cb.SelectedValuePath = "MaDatSan";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách đặt sân theo KH: " + ex.Message);
                cb.ItemsSource = new List<dynamic>();
            }
        }

        public void LoadNhanVien(ComboBox cb)
        {
            try
            {
                cb.ItemsSource = db.NHAN_VIEN.ToList();
                cb.DisplayMemberPath = "TenNV";
                cb.SelectedValuePath = "MaNV";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách nhân viên: " + ex.Message);
                cb.ItemsSource = new List<Model.NHAN_VIEN>();
            }
        }

        public dynamic LayThongTinDatSan(string maDat)
        {
            try
            {
                var datSan = db.LICH_DAT_SAN
                    .Include(d => d.KHACH_HANG)
                    .Include(d => d.SAN_BONG)
                    .FirstOrDefault(d => d.MaDatSan == maDat);

                if (datSan == null) return null;

                decimal soGioThue = 0;
                decimal thanhTien = 0;

                if (datSan.ThoiGianBatDau.HasValue && datSan.ThoiGianKetThuc.HasValue)
                {
                    soGioThue = (decimal)(datSan.ThoiGianKetThuc.Value - datSan.ThoiGianBatDau.Value).TotalHours;
                    thanhTien = soGioThue * (datSan.SAN_BONG?.DonGia ?? 0);
                }

                return new
                {
                    TenKH = datSan.KHACH_HANG?.TenKH ?? "",
                    TenSan = datSan.SAN_BONG?.TenSan ?? "",
                    ThoiGianBatDau = datSan.ThoiGianBatDau,
                    ThoiGianKetThuc = datSan.ThoiGianKetThuc,
                    DonGia = datSan.SAN_BONG?.DonGia ?? 0,
                    SoGioThue = soGioThue,
                    ThanhTien = thanhTien
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy thông tin đặt sân: " + ex.Message);
                return null;
            }
        }

        public void LoadCongNo(DataGrid dg, string timKiem = "")
        {
            try
            {
                var allKhachHang = db.KHACH_HANG.ToList();
                var allDatSan = db.LICH_DAT_SAN.Include(d => d.SAN_BONG).ToList();

                var khachHangCoCongNo = allKhachHang
                    .Where(kh => allDatSan.Any(d => d.MaKH == kh.MaKH && d.TinhTrang == false))
                    .ToList();

                var congNoList = new List<dynamic>();

                foreach (var kh in khachHangCoCongNo)
                {
                    var danhSachDatNo = allDatSan.Where(d => d.MaKH == kh.MaKH && d.TinhTrang == false).ToList();

                    decimal tongCongNo = 0;
                    foreach (var dat in danhSachDatNo)
                    {
                        if (dat.ThoiGianBatDau.HasValue && dat.ThoiGianKetThuc.HasValue && dat.SAN_BONG != null)
                        {
                            double soGioThue = (dat.ThoiGianKetThuc.Value - dat.ThoiGianBatDau.Value).TotalHours;
                            decimal donGia = dat.SAN_BONG.DonGia ?? 0;
                            tongCongNo += (decimal)soGioThue * donGia;
                        }
                    }

                    int soLanDatNo = danhSachDatNo.Count;

                    if (string.IsNullOrEmpty(timKiem) || kh.TenKH.ToLower().Contains(timKiem.ToLower()))
                    {
                        congNoList.Add(new
                        {
                            kh.MaKH,
                            kh.TenKH,
                            kh.SDT,
                            TongCongNo = tongCongNo,
                            SoLanDatNo = soLanDatNo
                        });
                    }
                }

                dg.ItemsSource = congNoList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải công nợ: " + ex.Message);
                dg.ItemsSource = new List<dynamic>();
            }
        }

        public void LoadBaoCao(DataGrid dg, DateTime? tuNgay, DateTime? denNgay, Label lblDoanhThu, Label lblCongNo)
        {
            if (!tuNgay.HasValue) tuNgay = DateTime.Now.AddDays(-30);
            if (!denNgay.HasValue) denNgay = DateTime.Now;

            try
            {
                var denNgayCuoiNgay = denNgay.Value.Date.AddDays(1).AddSeconds(-1);

                var allThanhToan = db.THANH_TOAN.ToList();
                var allDatSan = db.LICH_DAT_SAN.Include(d => d.SAN_BONG).ToList();

                var thanhToanTrongKhoang = allThanhToan
                    .Where(tt => tt.NgayTT >= tuNgay && tt.NgayTT <= denNgayCuoiNgay)
                    .ToList();

                // TẠO DANH SÁCH TẤT CẢ CÁC NGÀY TRONG KHOẢNG THỜI GIAN
                var allDates = new List<DateTime>();
                for (var date = tuNgay.Value.Date; date <= denNgay.Value.Date; date = date.AddDays(1))
                {
                    allDates.Add(date);
                }

                var baoCaoTheoNgay = allDates.Select(ngay => new
                {
                    Ngay = ngay.ToString("dd/MM/yyyy"),
                    // Số lượng thanh toán trong ngày
                    SoLuongTT = thanhToanTrongKhoang.Count(tt => tt.NgayTT.HasValue && tt.NgayTT.Value.Date == ngay),
                    // Doanh thu trong ngày
                    DoanhThu = thanhToanTrongKhoang.Where(tt => tt.NgayTT.HasValue && tt.NgayTT.Value.Date == ngay).Sum(tt => tt.SoTien) ?? 0,
                    // Số công nợ PHÁT SINH trong ngày (các đặt sân có ngày bắt đầu trong ngày đó và chưa thanh toán)
                    SoCongNo = allDatSan.Count(d =>
                        d.ThoiGianBatDau.HasValue &&
                        d.ThoiGianBatDau.Value.Date == ngay &&
                        d.TinhTrang == false),
                    // Tỷ lệ công nợ
                    TyLeCongNo = CalculateTyLeCongNoForDay(
                        thanhToanTrongKhoang.Where(tt => tt.NgayTT.HasValue && tt.NgayTT.Value.Date == ngay).Sum(tt => tt.SoTien) ?? 0,
                        ngay,
                        allDatSan)
                })
                .Where(x => x.SoLuongTT > 0 || x.SoCongNo > 0) // Chỉ hiện những ngày có dữ liệu
                .OrderBy(x => DateTime.ParseExact(x.Ngay, "dd/MM/yyyy", null))
                .ToList();

                dg.ItemsSource = baoCaoTheoNgay;

                // TÍNH TỔNG DOANH THU - tất cả thanh toán trong khoảng thời gian
                decimal tongDoanhThu = thanhToanTrongKhoang.Sum(tt => tt.SoTien) ?? 0;

                // TÍNH TỔNG CÔNG Nợ - tất cả đặt sân chưa thanh toán (không phân biệt ngày)
                decimal tongCongNo = 0;
                var danhSachDatNo = allDatSan.Where(dat => dat.TinhTrang == false).ToList();

                foreach (var dat in danhSachDatNo)
                {
                    if (dat.ThoiGianBatDau.HasValue && dat.ThoiGianKetThuc.HasValue && dat.SAN_BONG != null)
                    {
                        double soGioThue = (dat.ThoiGianKetThuc.Value - dat.ThoiGianBatDau.Value).TotalHours;
                        decimal donGia = dat.SAN_BONG.DonGia ?? 0;
                        tongCongNo += (decimal)soGioThue * donGia;
                    }
                }

                lblDoanhThu.Content = tongDoanhThu.ToString("N0");
                lblCongNo.Content = tongCongNo.ToString("N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo: " + ex.Message);
                dg.ItemsSource = new List<dynamic>();
                lblDoanhThu.Content = "0";
                lblCongNo.Content = "0";
            }
        }

        private string CalculateTyLeCongNoForDay(decimal doanhThu, DateTime ngay, List<Model.LICH_DAT_SAN> allDatSan)
        {
            // Tính tổng giá trị công nợ PHÁT SINH trong ngày
            decimal congNoPhatSinhTrongNgay = 0;
            var datNoTrongNgay = allDatSan
                .Where(d => d.ThoiGianBatDau.HasValue &&
                           d.ThoiGianBatDau.Value.Date == ngay &&
                           d.TinhTrang == false)
                .ToList();

            foreach (var dat in datNoTrongNgay)
            {
                if (dat.ThoiGianBatDau.HasValue && dat.ThoiGianKetThuc.HasValue && dat.SAN_BONG != null)
                {
                    double soGioThue = (dat.ThoiGianKetThuc.Value - dat.ThoiGianBatDau.Value).TotalHours;
                    decimal donGia = dat.SAN_BONG.DonGia ?? 0;
                    congNoPhatSinhTrongNgay += (decimal)soGioThue * donGia;
                }
            }

            if (doanhThu == 0 && congNoPhatSinhTrongNgay == 0) return "0%";
            if (doanhThu == 0) return "100%";

            decimal tyLe = (congNoPhatSinhTrongNgay / (doanhThu + congNoPhatSinhTrongNgay)) * 100;
            return tyLe.ToString("F1") + "%";
        }
    }
}
