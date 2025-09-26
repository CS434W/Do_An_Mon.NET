using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QLSanBong.ViewModel
{
    internal class QlySanBongViewModel
    {
        Model.Entities1 db = new Model.Entities1();

        public void ThemSANBONG(Model.SAN_BONG sv)
        {
            db.SAN_BONG.Add(sv);
            db.SaveChanges();
        }

        public void XoaSanBong(Model.SAN_BONG Xoa)
        {
            Model.SAN_BONG sb = db.SAN_BONG.Find(Xoa.MaSan);
            if (sb != null)
            {
                db.SAN_BONG.Remove(sb);
                db.SaveChanges();
            }
        }
        public void SuaSanBong(string oldMaSan, Model.SAN_BONG capNhat)
        {
            // Nếu mã sân không đổi, cập nhật trực tiếp
            if (string.Equals(oldMaSan, capNhat.MaSan, StringComparison.OrdinalIgnoreCase))
            {
                Model.SAN_BONG sb = db.SAN_BONG.Find(oldMaSan);
                if (sb != null)
                {
                    sb.TenSan = capNhat.TenSan;
                    sb.LoaiSan = capNhat.LoaiSan;
                    sb.DonGia = capNhat.DonGia;
                    sb.TrangThai = capNhat.TrangThai;
                    db.SaveChanges();
                }
                return;
            }

            // Đổi mã sân (khóa chính): tạo mới, cập nhật tham chiếu, xóa bản ghi cũ
            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    // Thêm bản ghi mới với mã mới
                    var sbMoi = new Model.SAN_BONG
                    {
                        MaSan = capNhat.MaSan,
                        TenSan = capNhat.TenSan,
                        LoaiSan = capNhat.LoaiSan,
                        DonGia = capNhat.DonGia,
                        TrangThai = capNhat.TrangThai
                    };
                    db.SAN_BONG.Add(sbMoi);
                    db.SaveChanges();

                    // Cập nhật các tham chiếu từ lịch đặt sân sang mã mới
                    var lichs = db.LICH_DAT_SAN.Where(l => l.MaSan == oldMaSan).ToList();
                    foreach (var lich in lichs)
                    {
                        lich.MaSan = capNhat.MaSan;
                    }
                    db.SaveChanges();

                    // Xóa bản ghi cũ
                    var sbCu = db.SAN_BONG.Find(oldMaSan);
                    if (sbCu != null)
                    {
                        db.SAN_BONG.Remove(sbCu);
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
        public void LoadSAN(DataGrid dg)//using System.Windows.Controls;
        {
            dg.ItemsSource = db.SAN_BONG.ToList();
        }
        //public void LoadLoaiSan(ComboBox cb)//using System.Windows.Controls;
        //{
        //    cb.ItemsSource = db.KHOAs.ToList();
        //    cb.DisplayMemberPath = "TenKhoa";
        //    cb.SelectedValuePath = "MaKhoa";
        //}
    }
}
