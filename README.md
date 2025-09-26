# Do_An_Mon.NET

WPF + Entity Framework 6 project.

## Local setup

This project uses SQL Server LocalDB with a named database `qlsanbong` (no .mdf checked in).

### One-time setup

1. Verify LocalDB exists:

   ```bash
   sqllocaldb info
   ```

2. Create DB and apply schema:

   ```bash
   scripts/setup_localdb.cmd
   ```

### Run

Open `QLSanBong.sln` in Visual Studio and run. Connection string targets `(localdb)\\MSSQLLocalDB` and `Initial Catalog=qlsanbong`.

### Notes

- `.vs/`, `bin/`, `obj/`, `*.mdf`, `*.ldf` are ignored by Git.
- To reset the DB:

  ```bash
  sqlcmd -S (localdb)\\MSSQLLocalDB -Q "IF DB_ID('qlsanbong') IS NOT NULL DROP DATABASE qlsanbong"
  ```
