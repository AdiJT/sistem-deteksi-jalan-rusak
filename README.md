# Download Model
- Download model dari https://drive.google.com/file/d/1rDFslKw7ozV3y6l2AhlxgL41LuhYjc6v/view?usp=sharing
- Di project DeteksiJalanRusak.Web klik kanan pilih Add > New Item.
- Di jendela yang terbuka, pilih di bagian samping kiri C# > ASP.NET Core > Web > ASP.NET. Kemudian klik file App Settings File
- Beri nama file baru appsettings.Development.json kemudian buka file paste :
  
  ```json
  {
    "FileConfiguration": {
      "FolderPath": "/file"
  },
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
      }
  },
    "AllowedHosts": "*"
  }
  ```
