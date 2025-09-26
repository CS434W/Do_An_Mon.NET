using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QLSanBong.Model;
using System.Data.Entity;
using System.Text.RegularExpressions;

namespace QLSanBong.ViewModel
{
	public class QuanLiNguoiDungViewModel : BaseViewModel
	{
		private Entities1 db = new Entities1();

		// Danh sách tài khoản nhân viên
		private ObservableCollection<TAI_KHOAN> _taiKhoans;
		public ObservableCollection<TAI_KHOAN> TaiKhoans
		{
			get => _taiKhoans;
			set { _taiKhoans = value; OnPropertyChanged(); }
		}

		private TAI_KHOAN _selectedTaiKhoan;
		public TAI_KHOAN SelectedTaiKhoan
		{
			get => _selectedTaiKhoan;
			set
			{
				_selectedTaiKhoan = value;
				OnPropertyChanged();

				if (value != null)
				{
					txtTenNV = value.NHAN_VIEN.FirstOrDefault()?.TenNV;
					txtSDT = value.NHAN_VIEN.FirstOrDefault()?.SDT;
					txtTenDangNhap = value.TenDangNhap;
					txtMatKhau = value.MatKhau;
				}
			}
		}

		// Các property bind với TextBox / PasswordBox
		private string _txtTenNV;
		public string txtTenNV { get => _txtTenNV; set { _txtTenNV = value; OnPropertyChanged(); } }

		private string _txtSDT;
		public string txtSDT { get => _txtSDT; set { _txtSDT = value; OnPropertyChanged(); } }

		private string _txtTenDangNhap;
		public string txtTenDangNhap { get => _txtTenDangNhap; set { _txtTenDangNhap = value; OnPropertyChanged(); } }

		private string _txtMatKhau;
		public string txtMatKhau { get => _txtMatKhau; set { _txtMatKhau = value; OnPropertyChanged(); } }

		// Commands
		public ICommand ThemCommand { get; set; }
		public ICommand SuaCommand { get; set; }
		public ICommand XoaCommand { get; set; }

		public QuanLiNguoiDungViewModel()
		{
			TaiKhoans = new ObservableCollection<TAI_KHOAN>(
				db.TAI_KHOAN.Where(t => t.VaiTro == "NhanVien").Include("NHAN_VIEN").ToList()
			);

			ThemCommand = new RelayCommand<object>(Them);
			SuaCommand = new RelayCommand<object>(Sua);
			XoaCommand = new RelayCommand<object>(Xoa);
		}

		private string GenerateNextId(string prefix, IQueryable<string> existingIds, int width)
		{
			// Lấy số lớn nhất với tiền tố cho trước, dạng PREFIX0001
			int maxNumber = 0;
			Regex rx = new Regex("^" + Regex.Escape(prefix) + "(\\d+)$");
			foreach (var id in existingIds)
			{
				if (string.IsNullOrWhiteSpace(id)) continue;
				var m = rx.Match(id);
				if (m.Success && int.TryParse(m.Groups[1].Value, out int n))
				{
					if (n > maxNumber) maxNumber = n;
				}
			}
			int next = maxNumber + 1;
			return prefix + next.ToString(new string('0', width));
		}

		private void Them(object obj)
		{
			if (string.IsNullOrEmpty(txtTenNV) || string.IsNullOrEmpty(txtTenDangNhap) || string.IsNullOrEmpty(txtMatKhau))
			{
				MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
				return;
			}

			// Không cho trùng tên đăng nhập
			if (db.TAI_KHOAN.Any(t => t.TenDangNhap == txtTenDangNhap))
			{
				MessageBox.Show("Tên đăng nhập đã tồn tại");
				return;
			}

			// Sinh mã tuần tự: TK0001, NV0001
			string newMaTK = GenerateNextId("TK", db.TAI_KHOAN.Select(x => x.MaTK), 4);
			string newMaNV = GenerateNextId("NV", db.NHAN_VIEN.Select(x => x.MaNV), 4);

			var tkMoi = new TAI_KHOAN
			{
				MaTK = newMaTK,
				TenDangNhap = txtTenDangNhap,
				MatKhau = txtMatKhau,
				VaiTro = "NhanVien",
			};

			var nv = new NHAN_VIEN
			{
				MaNV = newMaNV,
				TenNV = txtTenNV,
				SDT = txtSDT
			};

			// Liên kết thông qua navigation để EF chèn đúng thứ tự và chắc chắn ghi cả 2 bảng
			tkMoi.NHAN_VIEN.Add(nv);
			nv.TAI_KHOAN = tkMoi;

			// Thêm rõ ràng cả hai entity
			db.TAI_KHOAN.Add(tkMoi);
			db.NHAN_VIEN.Add(nv);
			db.SaveChanges();

			// Reload lại danh sách để cập nhật các cột bind phức tạp như NHAN_VIEN[0].TenNV
			TaiKhoans = new ObservableCollection<TAI_KHOAN>(
				db.TAI_KHOAN.Where(t => t.VaiTro == "NhanVien").Include("NHAN_VIEN").ToList()
			);
			MessageBox.Show("Thêm thành công");
		}

		private void Sua(object obj)
		{
			if (SelectedTaiKhoan == null)
			{
				MessageBox.Show("Vui lòng chọn tài khoản để sửa");
				return;
			}

			// Không cho trùng tên đăng nhập với tài khoản khác
			if (!string.Equals(SelectedTaiKhoan.TenDangNhap, txtTenDangNhap, StringComparison.OrdinalIgnoreCase)
				&& db.TAI_KHOAN.Any(t => t.TenDangNhap == txtTenDangNhap))
			{
				MessageBox.Show("Tên đăng nhập đã tồn tại");
				return;
			}

			SelectedTaiKhoan.TenDangNhap = txtTenDangNhap;
			SelectedTaiKhoan.MatKhau = txtMatKhau;

			var nv = SelectedTaiKhoan.NHAN_VIEN.FirstOrDefault();
			if (nv != null)
			{
				nv.TenNV = txtTenNV;
				nv.SDT = txtSDT;
			}

			db.SaveChanges();
			// Reload để DataGrid phản ánh thay đổi
			TaiKhoans = new ObservableCollection<TAI_KHOAN>(
				db.TAI_KHOAN.Where(t => t.VaiTro == "NhanVien").Include("NHAN_VIEN").ToList()
			);
			MessageBox.Show("Sửa thành công");
		}

		private void Xoa(object obj)
		{
			if (SelectedTaiKhoan == null)
			{
				MessageBox.Show("Vui lòng chọn tài khoản để xóa");
				return;
			}

			db.NHAN_VIEN.RemoveRange(SelectedTaiKhoan.NHAN_VIEN);
			db.TAI_KHOAN.Remove(SelectedTaiKhoan);
			db.SaveChanges();

			TaiKhoans = new ObservableCollection<TAI_KHOAN>(
				db.TAI_KHOAN.Where(t => t.VaiTro == "NhanVien").Include("NHAN_VIEN").ToList()
			);
			MessageBox.Show("Xóa thành công");
		}
	}
}
