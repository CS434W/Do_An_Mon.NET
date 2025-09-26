using System.Linq;

namespace QLSanBong.Model
{
	public partial class TAI_KHOAN
	{
		public string NhanVienTen
		{
			get { return NHAN_VIEN?.FirstOrDefault()?.TenNV; }
		}

		public string NhanVienSDT
		{
			get { return NHAN_VIEN?.FirstOrDefault()?.SDT; }
		}
	}
} 