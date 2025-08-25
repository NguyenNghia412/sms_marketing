```
dotnet ef migrations add InitMigration --project thongbao.be.infrastructure.data --startup-project thongbao.be --output-dir Migrations
dotnet ef database update --project thongbao.be.infrastructure.data --startup-project thongbao.be --output-dir Migrations
```