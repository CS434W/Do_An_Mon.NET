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
    /// Interaction logic for QuanLySanBong.xaml
    /// </summary>
    public partial class QuanLySanBong : Window
    {
        public QuanLySanBong()
        {
            InitializeComponent();
        }
        ViewModel.QlySanBongViewModel sbvm = new ViewModel.QlySanBongViewModel();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Them_Click(object sender, RoutedEventArgs e)
        {
            Model.SAN_BONG sv = new Model.SAN_BONG
            {
                MaSan = txt_MaSan.Text,
                TenSan = txt_TenSan.Text,
                LoaiSan = cb_LoaiSan.Text,
                DonGia = decimal.Parse(txt_DonGia.Text),
                TrangThai = cb_trangThai.SelectedIndex == 1
            };
            sbvm.ThemSANBONG(sv);
            sbvm.LoadSAN(dataGrid_QLSB);
            MessageBox.Show("Thêm sinh viên thành công");

        }

        private void btn_Sua_Click(object sender, RoutedEventArgs e)
        {
            {
                Model.SAN_BONG svchon = dataGrid_QLSB.SelectedItem as Model.SAN_BONG;
                if (svchon == null)
                {
                    MessageBox.Show("Hãy chọn sân cần sửa");
                    return;
                }
                Model.SAN_BONG sv = new Model.SAN_BONG
                {
                    MaSan = svchon.MaSan,
                    TenSan = txt_TenSan.Text,
                    LoaiSan = cb_LoaiSan.Text,
                    DonGia = decimal.Parse(txt_DonGia.Text),
                    TrangThai = cb_trangThai.SelectedIndex == 1
                };
                sbvm.SuaSanBong(sv);
                sbvm.LoadSAN(dataGrid_QLSB);
                MessageBox.Show("Sửa sân bóng thành công");
            }

        }

        private void btn_Xoa_Click(object sender, RoutedEventArgs e)
        {
            Model.SAN_BONG svchon = dataGrid_QLSB.SelectedItem as Model.SAN_BONG;
            if (svchon == null)
            {
                MessageBox.Show("Hãy chọn sân bóng cần xoá");
                return;
            }
            sbvm.XoaSanBong(svchon);
            sbvm.LoadSAN(dataGrid_QLSB);
            MessageBox.Show("Xoá sân bóng thành công");

        }

        private void dataGrid_QLSB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.SAN_BONG sv = dataGrid_QLSB.SelectedItem as Model.SAN_BONG;
            if (sv == null) return;
            txt_MaSan.Text = sv.MaSan;
            txt_TenSan.Text = sv.TenSan;
            txt_DonGia.Text = sv.DonGia.ToString();
            cb_LoaiSan.Text = sv.LoaiSan;
            if (sv.TrangThai == true)
                cb_trangThai.SelectedIndex = 1; // Hoạt động
            else
                cb_trangThai.SelectedIndex = 0; // Không hoạt động
        }
    }
}
