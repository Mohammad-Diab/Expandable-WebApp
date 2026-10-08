# Expandable WebApp

A users grid that each client can **customize with a plugin DLL, without touching the core product**. The core shows a list of users; a client that needs other data or other columns gets a small plugin. Copy the plugin's DLL into the `CustomDlls` folder and the grid changes on the next refresh; remove it and the grid is back to normal, while the app keeps running.

Written in January 2020 with ASP.NET Core 3.0: a Web API, a Razor Pages site with a jQuery grid, and SQL Server.

## How it works

- **Shared** is the contract. `IUser` loads the users (`GetAllUsers`), describes their columns (`GetProperties`) and returns a page (`UsersPageGet`). A `Property` is one column: name, label, order, type (hidden, text, number) and whether it can be filtered.
- **MappingLibraries** decides which class plays `IUser`. By default it is `MyAppLogic.BasicUser`. `Main` reads every DLL in `CustomDlls`, and a class there that derives from `Shared.IUser` takes its place. A `FileSystemWatcher` on the folder resets the mapping when a DLL is added, removed or renamed, so the next request uses the new class. `Main.Init()` starts it from `MyAppAPI\Startup.cs`.
- **MyAppAPI** has one action, `GET /MyApp/GetUsers?PageNumber=1&Filter=`. It creates the user class through `Main` and returns a page of users together with their `properties`.
- **MyAppSite** draws the grid in `wwwroot\js\site.js` from those `properties`: which columns, their labels, order and type. A plugin's new column needs no change in the site.

`CustomDlls` sits next to the folder the API runs from: for the release that is `Expandable-WebApp\CustomDlls`, and when running from source it is `src\MyAppAPI\bin\Debug\CustomDlls`. The API creates it when it starts.

## Layout

| Folder | What it is |
|---|---|
| `src\Shared` | The contract: `IUser`, `Property`, `IListViewer<T>` |
| `src\MappingLibraries` | Loads the plugins from `CustomDlls` and watches the folder |
| `src\MyAppLogic` | `BasicUser`, the default users |
| `src\MyAppDB` | SQL Server access (`DatabaseConnection`) |
| `src\MyAppAPI` | The Web API (port 50754) |
| `src\MyAppSite` | The site with the Users grid (port 50756) |
| `plugins\ClientCustomization` | Shows only Full Name and Alias name |
| `plugins\ClientCustomization02` | Adds an Alias name column to all the usual columns |
| `plugins\UserCustum` | The external example: its own solution, built against the core's DLLs only, with its own SQL query. Shows Full Name and Alias name |
| `database` | `ExpandableWebAppDB.sql` (the `tblUsers` table with 200 made-up users), `Setup-Database.cmd`, and `data.sql` (the first generated set of the same users, in MySQL syntax) |

## Write a plugin

1. Create a class library for `netcoreapp3.0` and reference the core's `Shared.dll` (and `MyAppDB.dll` if it reads the database). `plugins\UserCustum` does exactly this, through `HintPath` references, with no project reference to the core.
2. Add a public class that derives from `Shared.IUser` and override what the client needs: `GetAllUsers` for the data, `GetProperties` for the columns, `UsersPageGet` for paging and filtering.
3. Build it and copy the DLL (only the DLL) into `CustomDlls`. Refresh the Users page.

To change a plugin that is already loaded, restart the API after copying the new build: a loaded assembly stays loaded.

## Run the release

Needs Windows 10 or 11, the **ASP.NET Core Runtime 8.0 or later** ([download](https://dotnet.microsoft.com/download/dotnet), "ASP.NET Core Runtime", Windows x64) and **SQL Server Express LocalDB** (run the [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads) installer, choose **Download Media**, then **LocalDB**, and run `SqlLocalDB.msi`). The code targets .NET Core 3.0, which is out of support; `Start.cmd` lets it run on the newer runtime you have.

1. From the [Releases](https://github.com/Mohammad-Diab/Expandable-WebApp/releases) page, download `Expandable-WebApp-v1.0.0-win.7z` and the plugin archives you want, and extract them with [7-Zip](https://www.7-zip.org).
2. In the `Expandable-WebApp` folder, run `Setup-Database.cmd`. It creates the `ExpandableWebAppDB` database on LocalDB.
3. Run `Start.cmd`. It starts the API on `http://localhost:50754` and the site on `http://localhost:50756`, and opens the Users page.
4. While it runs, copy a plugin DLL into `Expandable-WebApp\CustomDlls` and refresh the page. Delete it and refresh again.

The site calls the API at `http://localhost:50754` (`ApiUrl` in `Client\wwwroot\js\site.js`), so keep that port.

## Run from source

Needs the .NET SDK 8.0 or later (it builds the `netcoreapp3.0` projects with an out-of-support warning) and SQL Server Express LocalDB.

```
database\Setup-Database.cmd
dotnet build Expandable-WebApp.sln
set DOTNET_ROLL_FORWARD=Major
start dotnet run --no-build --project src\MyAppAPI -- --urls http://localhost:50754
start dotnet run --no-build --project src\MyAppSite -- --urls http://localhost:50756
```

Open `http://localhost:50756/Users`, then copy for example `plugins\ClientCustomization\bin\Debug\netcoreapp3.0\ClientCustomization.dll` into `src\MyAppAPI\bin\Debug\CustomDlls`.

## Database

`Setup-Database.cmd` runs `ExpandableWebAppDB.sql` on `(localdb)\MSSQLLocalDB` with your Windows account (`Setup-Database.cmd -Server ".\SQLEXPRESS"` for SQL Server Express). The API reads its connection string from `ConnectionStrings:ExpandableWebAppDB` in `src\MyAppAPI\appsettings.json` (`Server\appsettings.json` in the release). For another server, change it there or set the `ConnectionStrings__ExpandableWebAppDB` environment variable; keep passwords out of the repository.

To remove the database: `sqlcmd -S "(localdb)\MSSQLLocalDB" -E -Q "DROP DATABASE [ExpandableWebAppDB]"`

## Known limits

- The Add and Delete buttons on the Users page are not wired to the API yet.
- The filter searches the full name only.
- The API accepts calls from any origin, so the site can call it from another port. Use it for demos, not on a public server.

## History

The first commit is the January 2020 code. The commits after it add `.gitignore`, the database setup script, and this README and license.

## License

[MIT](LICENSE)
