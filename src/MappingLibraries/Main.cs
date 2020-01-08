using Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace MappingLibraries
{
    public static class Main
    {
        static string libDirectory;
        static string LibDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(libDirectory))
                {
                    DirectoryInfo di = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent;
                    libDirectory = Path.Combine(di.FullName, "CustomDlls");
                }
                return libDirectory;
            }
        }

        static FileSystemWatcher FileSystemWatcher;


        static public void Init()
        {

            if (FileSystemWatcher == null)
            {
                Directory.CreateDirectory(LibDirectory);
                FileSystemWatcher = new FileSystemWatcher(LibDirectory);
                FileSystemWatcher.EnableRaisingEvents = true;
                FileSystemWatcher.Created += FileSystemWatcher_Created;
                FileSystemWatcher.Deleted += FileSystemWatcher_Created;
                FileSystemWatcher.Renamed += FileSystemWatcher_Created;
            }

        }

        // Dictionary Value
        // Item1 represent Library Assembly Name
        // Item2 represent Class Name
        // Item3 represent Class Full Name
        static Dictionary<string, Tuple<string, string, string>> librarysMapping;

        static Dictionary<string, Tuple<string, string, string>> LibrarysMapping
        {
            get
            {
                if (librarysMapping is null)
                {
                    librarysMapping = new Dictionary<string, Tuple<string, string, string>>();
                    librarysMapping = ReadCustomLibraries();
                }
                return librarysMapping;
            }
            set
            {
                librarysMapping = value;
            }
        }

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            LibrarysMapping = null;
            IUser.Users = null;
            IUser.Properties = null;
        }

        private static Dictionary<string, Tuple<string, string, string>> ReadCustomLibraries()
        {
            var libPaths = Directory.GetFiles(LibDirectory, "*.dll");
            Dictionary<string, Tuple<string, string, string>> librarysMapping = GetBasicLibMapping();
            bool isLoaded = false;
            for (int i = 0; i < libPaths.Length; i++)
            {
                isLoaded = false;
                // Loaded from bytes, so the file stays free to be replaced or removed while the app runs
                Assembly assembly = Assembly.Load(File.ReadAllBytes(libPaths[i]));
                var types = assembly.GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    if (librarysMapping.ContainsKey(types[j].BaseType?.FullName ?? ""))
                    {
                        if (!isLoaded)
                        {
                            isLoaded = true;
                            if (!AssemblyLoadContext.Default.Assemblies.Any(item => item.FullName == assembly.FullName))
                            {
                                AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(File.ReadAllBytes(libPaths[i])));
                            }
                        }
                        librarysMapping[types[j].BaseType?.FullName] = Tuple.Create(assembly.GetName().Name, types[j].Name, types[j].FullName);
                    }
                }
            }
            return librarysMapping;
        }

        private static Dictionary<string, Tuple<string, string, string>> GetBasicLibMapping()
        {
            Dictionary<string, Tuple<string, string, string>> librarysMapping = new Dictionary<string, Tuple<string, string, string>>();
            librarysMapping.Add("Shared.IUser", Tuple.Create("MyAppLogic", "BasicUser", "MyAppLogic.BasicUser"));
            return librarysMapping;
        }

        static public IUser CreateUser()
        {
            var objHandler = Activator.CreateInstance(LibrarysMapping["Shared.IUser"].Item1, LibrarysMapping["Shared.IUser"].Item3);
            IUser user = (IUser)objHandler.Unwrap();
            return user;
        }

        static public IUser CreateUser(SqlDataReader reader)
        {
            var objHandler = Activator.CreateInstance(LibrarysMapping["Shared.IUser"].Item1, LibrarysMapping["Shared.IUser"].Item3, new object[] { reader });
            IUser user = (IUser)objHandler.Unwrap();
            return user;
        }

        static public Type GetViewerType(string key)
        {
            //return Type.GetType($"Shared.IListViewer[{LibrarysMapping["Shared.IUser"].Item3}]");

            //var u = CreateUser().GetType();
            //var mylistType = typeof(IListViewer<>).MakeGenericType(new[] { u });
            //return mylistType;
            throw new Exception();
        }

    }
}
