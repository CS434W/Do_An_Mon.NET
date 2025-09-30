using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QLSanBong.ViewModel
{
    class QuanLyThietBiVaBaoTriViewModel
    {
        Model.Entities1 db = new Model.Entities1();
        public void ThemThietBi(Model.THIET_BI thietbi)
        {
            try
            {
                db.THIET_BI.Add(thietbi);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm thiết bị: " + ex.Message);
            }
        }

        public void SuaThietBi(Model.THIET_BI thietbi)
        {
            try
            {
                Model.THIET_BI sua = db.THIET_BI.Find(thietbi.MaThietBi);
                if (sua != null)
                {
                    sua.TenThietBi = thietbi.TenThietBi;
                    sua.SoLuong = thietbi.SoLuong;
                    sua.Loai = thietbi.Loai;
                    sua.MaSan = thietbi.MaSan;
                    sua.NgayNhap = thietbi.NgayNhap;
                    sua.TinhTrang = thietbi.TinhTrang;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi sửa thiết bị: " + ex.Message);
            }
        }

        public void XoaThietBi(Model.THIET_BI thietbi)
        {
            try
            {
                Model.THIET_BI xoa = db.THIET_BI.Find(thietbi.MaThietBi);
                if (xoa != null)
                {
                    db.THIET_BI.Remove(xoa);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa thiết bị: " + ex.Message);
            }
        }

        public void ThemBaoTri(Model.BAO_TRI baotri)
        {
            try
            {
                db.BAO_TRI.Add(baotri);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm bảo trì: " + ex.Message);
            }
        }

        public void SuaBaoTri(Model.BAO_TRI baotri)
        {
            try
            {
                Model.BAO_TRI sua = db.BAO_TRI.Find(baotri.MaBaoTri);
                if (sua != null)
                {
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
            catch (Exception ex)
            {
                throw new Exception("Lỗi sửa bảo trì: " + ex.Message);
            }
        }

        public void XoaBaoTri(Model.BAO_TRI baotri)
        {
            try
            {
                Model.BAO_TRI xoa = db.BAO_TRI.Find(baotri.MaBaoTri);
                if (xoa != null)
                {
                    db.BAO_TRI.Remove(xoa);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa bảo trì: " + ex.Message);
            }
        }
        public void LoadThietBi(DataGrid dg)
        {
            dg.ItemsSource = db.THIET_BI.ToList();
        }




        public void LoadBaoTri(DataGrid dg)
        {
            dg.ItemsSource = db.BAO_TRI.ToList();
        }

        public void LoadMaSan(ComboBox cb)
        {
            cb.ItemsSource = db.SAN_BONG.ToList();
            cb.SelectedValuePath = "MaSan";
            cb.DisplayMemberPath = "MaSan";
        }

        public void LoadNhanVien(ComboBox cb)
        {
            cb.ItemsSource = db.NHAN_VIEN.ToList();
            cb.SelectedValuePath = "MaNV";
            cb.DisplayMemberPath = "TenNV"; // Hoặc "TenNV" nếu muốn hiển thị tên
        }

        public void LoadMaThietBi(ComboBox cb)
        {
            try
            {
                var danhSachThietBi = db.THIET_BI.ToList(); // Thay THIETBI bằng tên bảng thiết bị của bạn

                if (danhSachThietBi == null || !danhSachThietBi.Any())
                {
                    MessageBox.Show("Không có dữ liệu thiết bị!");
                    return;
                }

                cb.ItemsSource = danhSachThietBi;
                cb.SelectedValuePath = "MaThietBi";
                cb.DisplayMemberPath = "MaThietBi";

                if (cb.Items.Count > 0)
                    cb.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load mã thiết bị: " + ex.Message);
            }
        }
    }
}
