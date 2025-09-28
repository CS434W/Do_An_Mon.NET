using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QLSanBong.ViewModel
{
    class QuanLyThietBiVaBaoTriViewModel
    {
        Model.Entities1 db = new Model.Entities1();
        public void ThemThietBi(Model.THIET_BI thietbi)
        {
            db.THIET_BI.Add(thietbi);
            db.SaveChanges();
        }

        public void SuaThietBi(Model.THIET_BI thietbi)
        {
            Model.THIET_BI sua = db.THIET_BI.Find(thietbi.MaThietBi);
            if (sua != null)
            {
                sua.MaThietBi = thietbi.MaThietBi;
                sua.TenThietBi = thietbi.TenThietBi;
                sua.SoLuong = thietbi.SoLuong;
                sua.Loai = thietbi.Loai;
                sua.MaSan = thietbi.MaSan;
                sua.NgayNhap = thietbi.NgayNhap;
                sua.TinhTrang = thietbi.TinhTrang;
                db.SaveChanges();
            }
        }

        public void XoaThietBi(Model.THIET_BI thietbi)
        {
            Model.THIET_BI xoa = db.THIET_BI.Find(thietbi.MaThietBi);
            if (xoa != null)
            {

                if (xoa != null)
                {
                    db.THIET_BI.Remove(xoa);
                    db.SaveChanges();
                }
            }
        }
        public void LoadThietBi(DataGrid dg)
        {
            dg.ItemsSource = db.THIET_BI.ToList();
        }

        public void ThemBaoTri(Model.BAO_TRI baotri)
        {
            db.BAO_TRI.Add(baotri);
            db.SaveChanges();
        }

        public void SuaBaoTri(Model.BAO_TRI baotri)
        {
            Model.BAO_TRI sua = db.BAO_TRI.Find(baotri.MaBaoTri);
            if (sua != null)
            {
                sua.MaBaoTri = baotri.MaBaoTri;
                sua.MaThietBi = baotri.MaThietBi;
                sua.NgayBatDau = baotri.NgayBatDau;
                sua.NgayKetThuc = baotri.NgayKetThuc;
                sua.LoaiBaoTri = baotri.LoaiBaoTri;
                sua.ChiPhi = baotri.ChiPhi;
                sua.TrangThai = baotri.TrangThai;
                sua.NhanVienBT = baotri.NhanVienBT;
                db.SaveChanges();
            }
        }

        public void XoaBaoTri(Model.BAO_TRI baotri)
        {
            Model.BAO_TRI xoa = db.BAO_TRI.Find(baotri.MaBaoTri);
            if (xoa != null)
            {

                if (xoa != null)
                {
                    db.BAO_TRI.Remove(xoa);
                    db.SaveChanges();
                }
            }
        }

        public void LoadBaoTri(DataGrid dg)
        {
            dg.ItemsSource = db.BAO_TRI.ToList();
        }
    }
}
