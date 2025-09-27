using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QLSanBong.Model;
using QLSanBong.View;

namespace QLSanBong.ViewModel
{
    public class DangNhapViewModel : BaseViewModel
    {
        private string _tenDangNhap;
        public string TenDangNhap
        {
            get => _tenDangNhap;
            set { _tenDangNhap = value; OnPropertyChanged(); }
        }

        private string _thongBao;
        public string ThongBao
        {
            get => _thongBao;
            set { _thongBao = value; OnPropertyChanged(); }
        }

        public ICommand DangNhapCommand { get; }

        private Entities1 db;

        public DangNhapViewModel()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                db = new Entities1(); // dùng DbContext thật
            }

            DangNhapCommand = new RelayCommand<object>(DangNhap);
        }


        public void DangNhap(object parameter)
        {
            string matKhau = parameter as string;

            if (string.IsNullOrEmpty(TenDangNhap) || string.IsNullOrEmpty(matKhau))
            {
                ThongBao = "Vui lòng nhập đầy đủ thông tin.";
                return;
            }

            // Tài khoản admin cố định trong code
            if (TenDangNhap.Equals("admin", StringComparison.OrdinalIgnoreCase) && matKhau == "123456")
            {
                var adminAccount = new TAI_KHOAN
                {
                    TenDangNhap = "admin",
                    VaiTro = "Admin"
                };

                CurrentUser.User = adminAccount;

                var qlNguoiDungWindow = new QuanLiNguoiDung();
                qlNguoiDungWindow.Show();

                Application.Current.Windows
                           .OfType<Window>()
                           .SingleOrDefault(w => w.DataContext == this)?.Close();
                return;
            }

            var account = db.TAI_KHOAN
                            .FirstOrDefault(t => t.TenDangNhap == TenDangNhap && t.MatKhau == matKhau);

            if (account == null)
            {
                ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return;
            }

            // Mở MainWindow cho cả Admin và nhân viên
            CurrentUser.User = account;
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Đóng cửa sổ đăng nhập
            Application.Current.Windows
                       .OfType<Window>()
                       .SingleOrDefault(w => w.DataContext == this)?.Close();
        }
    }
}