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

        public void LoadLichSuDatSan(DataGrid dg, string maKH)
        {
            var lichSu = from lds in db.LICH_DAT_SAN
                         join sb in db.SAN_BONG on lds.MaSan equals sb.MaSan
                         where lds.MaKH == maKH
                         select new
                         {
                             MaDatSan = lds.MaDatSan,
                             TenSan = sb.TenSan,
                             ThoiGianBatDau = lds.ThoiGianBatDau,
                             ThoiGianKetThuc = lds.ThoiGianKetThuc,
                             TinhTrang = lds.TinhTrang
                         };
            dg.ItemsSource = lichSu.ToList();
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
