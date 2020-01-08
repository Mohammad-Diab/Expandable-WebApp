using MyAppDB;
using Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ClientCustomization
{
    public class CustomUser : IUser , ISerializable
    {
        public string Alias { get; set; }

        public override List<IUser> GetAllUsers()
        {

            string statment = "Select * from tblUsers";
            SqlDataReader reader = DatabaseConnection.Query(statment);
            Users = new List<IUser>();
            while (reader.Read())
            {
                Users.Add(new CustomUser(reader));
            }
            return Users;
        }

        public CustomUser()
        {
        }

        public CustomUser(SqlDataReader reader)
        {
            Uid = reader[reader.GetOrdinal("fldUid")].ToString();
            Alias = reader[reader.GetOrdinal("fldAlias")].ToString();
            FullName = reader[reader.GetOrdinal("fldFullName")].ToString();
        }

        public override List<Property> GetProperties()
        {
            if (Properties == null)
            {
                Properties = new List<Property>();
                Properties.Add(new Property("uid", "", 1, PropertyType.Hidden, false));
                Properties.Add(new Property("fullName", " Full Name", 2, PropertyType.String, true));
                Properties.Add(new Property("alias", "Alias name", 3, PropertyType.String, true));
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

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("uid", Uid);
            info.AddValue("fullName", Alias);
            info.AddValue("alias", FullName);
        }

    }
}
