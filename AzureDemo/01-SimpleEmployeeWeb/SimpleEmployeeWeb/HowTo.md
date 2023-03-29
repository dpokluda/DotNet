### Install libraries
For more information about [Client-side development in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/client-side/?view=aspnetcore-2.1)

Install LibMan CLI: [Use the LibMan command-line interface (CLI) with ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/client-side/libman/libman-cli?view=aspnetcore-2.1)

```
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
```

Restart your command prompt and then use the following commands:

```shell
libman install jquery --destination wwwroot/lib/jquery --files jquery.js --files jquery.min.js
libman install jquery-validate --destination wwwroot/lib/jquery-validate  --files jquery.validate.js --files jquery.validate.min.js
libman install jquery-validation-unobtrusive --destination wwwroot/lib/jquery-validation-unobtrusive  --files jquery.validate.unobtrusive.js  --files jquery.validate.unobtrusive.min.js
libman install twitter-bootstrap --destination wwwroot/lib/bootstrap
libman install bootstrap-icons --destination wwwroot/lib/bootstrap-icons
```