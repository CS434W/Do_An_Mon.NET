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
using System.Globalization;

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
            try
            {
                // Đổ dữ liệu bảng SAN_BONG ra DataGrid khi mở cửa sổ
                using (var ctx = new Model.Entities1())
                {
                    dataGrid_QLSB.ItemsSource = ctx.SAN_BONG.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi tải dữ liệu SAN_BONG");
            }
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
                // Parse Đơn giá linh hoạt (hỗ trợ cả dấu . và ,)
                string rawDonGia = txt_DonGia.Text.Trim();
                string normalizedDonGia = rawDonGia.Replace(".", string.Empty).Replace(",", ".");
                decimal donGia;
                if (!decimal.TryParse(normalizedDonGia, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out donGia))
                {
                    MessageBox.Show("Đơn giá không hợp lệ");
                    return;
                }

                string oldMaSan = svchon.MaSan;
                string newMaSan = txt_MaSan.Text.Trim();
                string tenSan = txt_TenSan.Text;
                string loaiSan = cb_LoaiSan.Text;
                bool? trangThai = cb_trangThai.SelectedIndex == 0; // 0 = Hoạt động

                using (var ctx = new Model.Entities1())
                {
                    var entity = ctx.SAN_BONG.Find(oldMaSan);
                    if (entity == null)
                    {
                        MessageBox.Show("Không tìm thấy sân để sửa");
                        return;
                    }

                    if (!string.Equals(oldMaSan, newMaSan, StringComparison.Ordinal))
                    {
                        // Đổi khóa chính: tạo bản ghi mới rồi xóa bản ghi cũ
                        var existedNew = ctx.SAN_BONG.Find(newMaSan);
                        if (existedNew != null)
                        {
                            MessageBox.Show("Mã sân mới đã tồn tại");
                            return;
                        }

                        var newEntity = new Model.SAN_BONG
                        {
                            MaSan = newMaSan,
                            TenSan = tenSan,
                            LoaiSan = loaiSan,
                            DonGia = donGia,
                            TrangThai = trangThai
                        };
                        ctx.SAN_BONG.Add(newEntity);
                        ctx.SAN_BONG.Remove(entity);
                    }
                    else
                    {
                        // Cập nhật thông tin khác
                        entity.TenSan = tenSan;
                        entity.LoaiSan = loaiSan;
                        entity.DonGia = donGia;
                        entity.TrangThai = trangThai;
                    }

                    ctx.SaveChanges();
                    dataGrid_QLSB.ItemsSource = ctx.SAN_BONG.ToList();
                }

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
                cb_trangThai.SelectedIndex = 0; // Hoạt động
            else
                cb_trangThai.SelectedIndex = 1; // Không hoạt động
        }
    }
}
