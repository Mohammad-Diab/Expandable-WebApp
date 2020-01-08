using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using MyAppDB;

namespace Shared
{
    public class IUser
    {
        public static List<IUser> Users { get; set; }

        public static List<Property> Properties { get; set; }

        //public static List<IUser> Users
        //{
        //    get
        //    {
        //        return users ?? throw new Exception();
        //    }
        //    set
        //    {
        //        users = value;
        //    }
        //}

        public string Uid { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public byte Age { get; set; }
        public string PhoneNumber { get; set; }

        public virtual List<IUser> GetAllUsers()
        {
            string statment = "Select * from tblUsers";
            SqlDataReader reader = DatabaseConnection.Query(statment);
            Users = new List<IUser>();
            while (reader.Read())
            {
                Users.Add(new IUser(reader));
            }
            return Users;
        }

        public IUser()
        {

        }

        public IUser(SqlDataReader reader)
        {
            Uid = reader[reader.GetOrdinal("fldUid")].ToString();
            UserName = reader[reader.GetOrdinal("fldUserName")].ToString();
            FullName = reader[reader.GetOrdinal("fldFullName")].ToString();
            Age = Converter.ToByte(reader[reader.GetOrdinal("fldAge")]);
            PhoneNumber = reader[reader.GetOrdinal("fldPhoneNumber")].ToString();
        }

        public virtual IUser GetUser(string Uid)
        {
            return Users.Find(item => item.Uid == Uid);
        }

        public virtual void AddUser(IUser user)
        {
            Users.Add(user);
        }

        public virtual List<Property> GetProperties()
        {
            if (Properties == null)
            {
                Properties = new List<Property>();
                Properties.Add(new Property("uid", "", 1, PropertyType.Hidden, false));
                Properties.Add(new Property("fullName", " Full Name", 2, PropertyType.String, true));
                Properties.Add(new Property("userName", "Username", 3, PropertyType.String, true));
                Properties.Add(new Property("age", "Age", 4, PropertyType.Number, true));
                Properties.Add(new Property("phoneNumber", "Phone Number", 5, PropertyType.String, true));
            }
            return Properties;
        }

        public virtual IListViewer<IUser> UsersPageGet(IUser helperObj, string Filter, int ItemPerPage, int PageNumber)
        {
            if (Users == null)
            {
                Users = helperObj.GetAllUsers();
            }
            var temp = Users.Where(item => item.FullName.Contains(Filter));
            var items = temp.Skip((PageNumber - 1) * ItemPerPage).Take(ItemPerPage).ToList();
            var properties = GetProperties();
            return new IListViewer<IUser>(items, properties, temp.Count(), ItemPerPage, PageNumber);
        }

        //public virtual string ss(IUser helperObj, string Filter, int ItemPerPage, int PageNumber)
        //{
        //    var obj = UsersPageGet(helperObj, Filter, ItemPerPage, PageNumber);

        //    return var s = JsonSerializer.Serialize(obj, obj.);
        //}


    }

}
