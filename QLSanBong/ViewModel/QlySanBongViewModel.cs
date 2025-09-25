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
        public void SuaSanBong(Model.SAN_BONG CapNhap)
        {
            Model.SAN_BONG sb = db.SAN_BONG.Find(CapNhap.MaSan);
            if (sb != null)
            {
                sb.TenSan = CapNhap.TenSan;
                sb.LoaiSan = CapNhap.LoaiSan;
                sb.DonGia = CapNhap.DonGia;
                sb.TrangThai = CapNhap.TrangThai;
                db.SaveChanges();
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
