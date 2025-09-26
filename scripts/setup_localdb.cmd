@echo off
setlocal enabledelayedexpansion

REM Determine repo root (this script is in scripts/)
set SCRIPT_DIR=%~dp0
for %%I in ("%SCRIPT_DIR%..") do set REPO_ROOT=%%~fI

set SERVER=(localdb)\MSSQLLocalDB
set DBNAME=qlsanbong
set SCHEMA_FILE=%REPO_ROOT%\QLSanBong\DB\qlsanbong_schema.sql

where sqlcmd >nul 2>nul
if errorlevel 1 (
  echo ERROR: sqlcmd not found. Install SQL Server Command Line Utilities or use SSMS to run the schema.
  exit /b 1
)

echo Creating database if not exists: %DBNAME% on %SERVER%
sqlcmd -S %SERVER% -Q "IF DB_ID('%DBNAME%') IS NULL CREATE DATABASE %DBNAME%" || goto :error

echo Applying schema: %SCHEMA_FILE%
sqlcmd -S %SERVER% -d %DBNAME% -i "%SCHEMA_FILE%" || goto :error

echo Done.
exit /b 0

:error
echo Failed.
exit /b 1


