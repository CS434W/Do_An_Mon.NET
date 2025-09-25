-- Create database if not exists (for LocalDB attach scenarios, you usually attach MDF instead)
-- Use only if you run against an existing server database
-- CREATE DATABASE qlsanbong;
-- GO

-- Use your database
-- USE qlsanbong;
-- GO

-- Core tables used by the app
IF OBJECT_ID(N'dbo.TAI_KHOAN', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.TAI_KHOAN
	(
		MaTK          NVARCHAR(50)  NOT NULL PRIMARY KEY,
		TenDangNhap   NVARCHAR(255) NOT NULL,
		MatKhau       NVARCHAR(255) NULL,
		VaiTro        NVARCHAR(50)  NULL
	);
	-- Unique username to prevent duplicates per app logic
	CREATE UNIQUE INDEX UX_TAI_KHOAN_TenDangNhap ON dbo.TAI_KHOAN(TenDangNhap);
END
GO

IF OBJECT_ID(N'dbo.NHAN_VIEN', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.NHAN_VIEN
	(
		MaNV     NVARCHAR(50)  NOT NULL PRIMARY KEY,
		TenNV    NVARCHAR(255) NULL,
		GioiTinh BIT           NULL,
		SDT      NVARCHAR(20)  NULL,
		MaTK     NVARCHAR(50)  NULL
	);
END
GO

-- Foreign key relation from NHAN_VIEN -> TAI_KHOAN
IF NOT EXISTS (
	SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_NhanVien_TaiKhoan'
)
BEGIN
	ALTER TABLE dbo.NHAN_VIEN
	ADD CONSTRAINT FK_NhanVien_TaiKhoan
	FOREIGN KEY (MaTK)
	REFERENCES dbo.TAI_KHOAN(MaTK)
	ON UPDATE NO ACTION
	ON DELETE NO ACTION;
END
GO

-- Optional sample admin account for quick testing (admin/123456)
IF NOT EXISTS (SELECT 1 FROM dbo.TAI_KHOAN WHERE TenDangNhap = N'admin')
BEGIN
	INSERT INTO dbo.TAI_KHOAN (MaTK, TenDangNhap, MatKhau, VaiTro)
	VALUES (N'TK_ADMIN', N'admin', N'123456', N'Admin');
END
GO 