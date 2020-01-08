using MyAppDB;
using Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;

namespace ClientCustomization02
{
    [DataContract]
    public class AnotherCustomUser:IUser
    {
        public string Alias { get; set; }

        public override List<IUser> GetAllUsers()
        {
            var baseList = base.GetAllUsers();
            var anotherCustomUserList = new List<IUser>();
            string statment = "Select fldUid, fldAlias from tblUsers";
            SqlDataReader reader = DatabaseConnection.Query(statment);
            while (reader.Read())
            {
                anotherCustomUserList.Add(new AnotherCustomUser(reader));
            }
            Users = (from baseUser in baseList
                     join user in anotherCustomUserList on baseUser.Uid equals user.Uid
                     select new AnotherCustomUser() { Uid = baseUser.Uid, UserName = baseUser.UserName, FullName = baseUser.FullName, Age = baseUser.Age, PhoneNumber = baseUser.PhoneNumber, Alias = (user as AnotherCustomUser).Alias } as IUser).ToList();
            return Users;
        }

        public AnotherCustomUser()
        {
        }

        public AnotherCustomUser(SqlDataReader reader)
        {
            Uid = reader[reader.GetOrdinal("fldUid")].ToString();
            Alias = reader[reader.GetOrdinal("fldAlias")].ToString();
        }

        public override List<Property> GetProperties()
        {
            if (Properties == null)
            {
                Properties = new List<Property>();
                Properties.Add(new Property("uid", "", 1, PropertyType.Hidden, false));
                Properties.Add(new Property("fullName", " Full Name", 2, PropertyType.String, true));
                Properties.Add(new Property("userName", "Username", 3, PropertyType.String, true));
                Properties.Add(new Property("alias", "Alias name", 4, PropertyType.String, true));
                Properties.Add(new Property("age", "Age", 5, PropertyType.Number, true));
                Properties.Add(new Property("phoneNumber", "Phone Number", 6, PropertyType.String, true));
            }
            return Properties;
        }

        public override IListViewer<IUser> UsersPageGet(IUser helperObj, string Filter, int ItemPerPage, int PageNumber)
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

    }
}
