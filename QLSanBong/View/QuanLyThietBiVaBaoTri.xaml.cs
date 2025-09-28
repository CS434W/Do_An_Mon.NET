using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QLSanBong.View
{
    /// <summary>
    /// Interaction logic for QuanLyThietBiVaBaoTri.xaml
    /// </summary>
    public partial class QuanLyThietBiVaBaoTri : Window
    {
        public QuanLyThietBiVaBaoTri()
        {
            InitializeComponent();
        }
        ViewModel.QuanLyThietBiVaBaoTriViewModel thietbibaotri = new ViewModel.QuanLyThietBiVaBaoTriViewModel();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            thietbibaotri.LoadThietBi(DG_TB);
            thietbibaotri.LoadBaoTri(DG_BT);
        }

        private void Button_Them(object sender, RoutedEventArgs e)
        {
            Model.THIET_BI them = new Model.THIET_BI();
            //table.Id = int.Parse(txt_Id.Text);
            them.MaThietBi = txtMaThietBi.Text;
            them.TenThietBi = txtTenThietBi.Text;
            them.SoLuong = int.Parse(txtSoLuong.Text);
            them.Loai = txtLoai.Text;
            them.NgayNhap = DateTime.Parse(dpNgayNhap.Text);
            them.TinhTrang = cbTinhTrang.SelectedIndex == 1;
            them.MaSan = txtMaSan.Text;

            thietbibaotri.ThemThietBi(them);
            thietbibaotri.LoadThietBi(DG_TB);
            MessageBox.Show("Thêm thiết bị thành công");
        }

        private void Button_Sua(object sender, RoutedEventArgs e)
        {
            Model.THIET_BI sua = DG_TB.SelectedItem as Model.THIET_BI;
            if (sua == null)
            {
                MessageBox.Show("Hãy chọn thiết bị cần sửa");
                return;
            }
            Model.THIET_BI sua1 = new Model.THIET_BI()
            {
                MaThietBi = txtMaThietBi.Text,
                TenThietBi = txtTenThietBi.Text,
                SoLuong = int.Parse(txtSoLuong.Text),
                Loai = txtLoai.Text,
                NgayNhap = DateTime.Parse(dpNgayNhap.Text),
                TinhTrang = cbTinhTrang.SelectedIndex == 1,
                MaSan = txtMaSan.Text
            };
            thietbibaotri.SuaThietBi(sua1);
            thietbibaotri.LoadThietBi(DG_TB);
            MessageBox.Show("Sửa thiết bị thành công");
        }

        private void Button_Xoa(object sender, RoutedEventArgs e)
        {
            Model.THIET_BI xoa = DG_TB.SelectedItem as Model.THIET_BI;
            if (xoa == null)
            {
                MessageBox.Show("Hãy chọn thiết bị cần xóa");
                return;
            }
            thietbibaotri.XoaThietBi(xoa);
            thietbibaotri.LoadThietBi(DG_TB);
            MessageBox.Show("Xóa thiết bị thành công");
        }

        private void Button_ThemBT(object sender, RoutedEventArgs e)
        {
            Model.BAO_TRI them = new Model.BAO_TRI();
            //table.Id = int.Parse(txt_Id.Text);
            them.MaBaoTri = txtMaBaoTri.Text;
            them.MaThietBi = cbMaThietBi.Text;
            them.NgayBatDau = DateTime.Parse(dpNgayBatDau.Text);
            them.NgayKetThuc = DateTime.Parse(dpNgayKetThuc.Text);
            them.LoaiBaoTri = txtLoaiBaoTri.Text;
            them.ChiPhi = decimal.Parse(txtChiPhi.Text);
            them.TrangThai = cbTrangThai.SelectedIndex == 1;
            them.NhanVienBT = txtNhanVienBT.Text;

            thietbibaotri.ThemBaoTri(them);
            thietbibaotri.LoadBaoTri(DG_BT);
            MessageBox.Show("Thêm bảo trì thành công");
        }

        private void Button_SuaBT(object sender, RoutedEventArgs e)
        {
            Model.BAO_TRI sua = DG_TB.SelectedItem as Model.BAO_TRI;
            if (sua == null)
            {
                MessageBox.Show("Hãy chọn bảo trì cần sửa");
                return;
            }
            Model.BAO_TRI sua1 = new Model.BAO_TRI()
            {
                MaBaoTri = txtMaBaoTri.Text,
                MaThietBi = cbMaThietBi.Text,
                NgayBatDau = DateTime.Parse(dpNgayBatDau.Text),
                NgayKetThuc = DateTime.Parse(dpNgayKetThuc.Text),
                LoaiBaoTri = txtLoaiBaoTri.Text,
                ChiPhi = decimal.Parse(txtChiPhi.Text),
                TrangThai = cbTrangThai.SelectedIndex == 1,
                NhanVienBT = txtNhanVienBT.Text
            };
            thietbibaotri.SuaBaoTri(sua1);
            thietbibaotri.LoadBaoTri(DG_BT);
            MessageBox.Show("Sửa bảo trì thành công");
        }

        private void Button_XoaBT(object sender, RoutedEventArgs e)
        {
            Model.BAO_TRI xoa = DG_TB.SelectedItem as Model.BAO_TRI;
            if (xoa == null)
            {
                MessageBox.Show("Hãy chọn bảo trì cần xóa");
                return;
            }
            thietbibaotri.XoaBaoTri(xoa);
            thietbibaotri.LoadBaoTri(DG_BT);
            MessageBox.Show("Xóa bảo trì thành công");
        }
    }
}
