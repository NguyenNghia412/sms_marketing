Mở terminal tại folder root và chạy lệnh sau để tạo migration và cập nhật database:
```
dotnet ef migrations add InitMigration --project thongbao.be.infrastructure.data --startup-project thongbao.be --output-dir Migrations
dotnet ef database update --project thongbao.be.infrastructure.data --startup-project thongbao.be
```

```
dotnet user-secrets set "AuthServer:MS:ClientId" "" --project thongbao.be
dotnet user-secrets set "AuthServer:MS:ClientSecret" "" --project thongbao.be
```