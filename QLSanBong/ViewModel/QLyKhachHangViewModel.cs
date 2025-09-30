using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QLSanBong.ViewModel
{
    internal class QLyKhachHangViewModel
    {
        Model.Entities1 db = new Model.Entities1();
        public void ThemKHACHHANG(Model.KHACH_HANG kh)
        {
            db.KHACH_HANG.Add(kh);
            db.SaveChanges();
        }
        public void XoaKhachHang(Model.KHACH_HANG xoa)
        {
            Model.KHACH_HANG kh = db.KHACH_HANG.Find(xoa.MaKH);
            if (kh != null)
            {
                // Kiểm tra xem khách hàng có lịch đặt sân không
                bool coLichDat = db.LICH_DAT_SAN.Any(lds => lds.MaKH == kh.MaKH);
                if (coLichDat)
                {
                    throw new Exception("Không thể xóa khách hàng vì có lịch đặt sân liên quan");
                }

                db.KHACH_HANG.Remove(kh);
                db.SaveChanges();
            }
        }
        public void SuaKhachHang(string oldMaKH, Model.KHACH_HANG capNhat)
        {
            // Nếu mã KH không đổi, cập nhật trực tiếp
            if (string.Equals(oldMaKH, capNhat.MaKH, StringComparison.OrdinalIgnoreCase))
            {
                Model.KHACH_HANG kh = db.KHACH_HANG.Find(oldMaKH);
                if (kh != null)
                {
                    kh.TenKH = capNhat.TenKH;
                    kh.SDT = capNhat.SDT;
                    kh.GhiChu = capNhat.GhiChu;
                    db.SaveChanges();
                }
                return;
            }

            // Đổi mã KH (khóa chính): tạo mới, cập nhật tham chiếu, xóa bản ghi cũ
            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    // Thêm bản ghi mới với mã mới
                    var khMoi = new Model.KHACH_HANG
                    {
                        MaKH = capNhat.MaKH,
                        TenKH = capNhat.TenKH,
                        SDT = capNhat.SDT,
                        GhiChu = capNhat.GhiChu
                    };
                    db.KHACH_HANG.Add(khMoi);
                    db.SaveChanges();

                    // Cập nhật các tham chiếu từ lịch đặt sân sang mã mới
                    var lichs = db.LICH_DAT_SAN.Where(l => l.MaKH == oldMaKH).ToList();
                    foreach (var lich in lichs)
                    {
                        lich.MaKH = capNhat.MaKH;
                    }
                    db.SaveChanges();

                    // Xóa bản ghi cũ
                    var khCu = db.KHACH_HANG.Find(oldMaKH);
                    if (khCu != null)
                    {
                        db.KHACH_HANG.Remove(khCu);
                        db.SaveChanges();
                    }

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public void LoadKHACHHANG(DataGrid dg)
        {
            dg.ItemsSource = db.KHACH_HANG.ToList();
        }

        //public void LoadLichSuDatSan(DataGrid dg, string maKH)
        //{
        //    try {
        //        var lichSu = from lds in db.LICH_DAT_SAN
        //                     join sb in db.SAN_BONG on lds.MaSan equals sb.MaSan
        //                     where lds.MaKH == maKH
        //                     select new
        //                     {
        //                         MaDatSan = lds.MaDatSan,
        //                         TenSan = sb.TenSan,
        //                         ThoiGianBatDau = lds.ThoiGianBatDau,
        //                         ThoiGianKetThuc = lds.ThoiGianKetThuc,
        //                         TinhTrang = lds.TinhTrang == true? "Đang hoạt động" : "Đã hủy"
        //                     };
        //        dg.ItemsSource = lichSu.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.MessageBox.Show("Lỗi khi tải lịch sử đặt sân: " + ex.Message);
        //    }
        //}

        //public void KiemTraPropertiesThucTe()
        //{
        //    try
        //    {
        //        var properties = typeof(Model.LICH_DAT_SAN).GetProperties();
        //        string result = "Properties trong LICH_DAT_SAN:\n";
        //        foreach (var prop in properties)
        //        {
        //            result += $"{prop.Name}\n";
        //        }
        //        System.Windows.MessageBox.Show(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.MessageBox.Show("Lỗi: " + ex.Message);
        //    }
        //}
        public void LoadLichSuDatSan(DataGrid dg)
        {
            try
            {
                dg.AutoGenerateColumns = true;

                // THỬ CÁC TÊN PROPERTIES KHÁC NHAU
                var lichSu = from lds in db.LICH_DAT_SAN
                             join sb in db.SAN_BONG on lds.MaSan equals sb.MaSan
                             join kh in db.KHACH_HANG on lds.MaKH equals kh.MaKH
                             select new
                             {
                                 // Thử các tên khác nhau
                                 MaDat = lds.MaDatSan,    // hoặc lds.MaDat
                                 San = sb.TenSan,         // hoặc sb.TenSan
                                 BatDau = lds.ThoiGianBatDau, // hoặc lds.BatDau, lds.ThoiGianBatDau
                                 KetThuc = lds.ThoiGianKetThuc, // hoặc lds.KetThuc, lds.ThoiGianKetThuc
                                 TrangThai = lds.TinhTrang == true ? "Đang hoạt động" : "Đã hủy" // hoặc lds.TrangThai
                             };

                var result = lichSu.ToList();
                dg.ItemsSource = result;

                System.Windows.MessageBox.Show($"Đã tải {result.Count} lịch đặt sân");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi tải lịch sử: {ex.Message}");

                // THỬ PHIÊN BẢN ĐƠN GIẢN
                try
                {
                    var simpleList = db.LICH_DAT_SAN.Take(5).ToList(); // Lấy 5 bản ghi đầu
                    dg.ItemsSource = simpleList;
                    System.Windows.MessageBox.Show($"Đã tải {simpleList.Count} bản ghi đơn giản");
                }
                catch (Exception ex2)
                {
                    System.Windows.MessageBox.Show($"Lỗi phiên bản đơn giản: {ex2.Message}");
                }
            }
        }
        public void TimKiemKhachHang(DataGrid dg, string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                LoadKHACHHANG(dg);
                return;
            }

            var ketQua = db.KHACH_HANG
                .Where(kh => kh.TenKH.Contains(tuKhoa) ||
                             kh.MaKH.Contains(tuKhoa) ||
                             kh.SDT.Contains(tuKhoa))
                .ToList();
            dg.ItemsSource = ketQua;
        }

    }
}
