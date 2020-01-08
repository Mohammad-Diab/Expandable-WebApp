using Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyAppLogic
{
    public class BasicUser : IUser
    {

        public override List<IUser> GetAllUsers()
        {
            List<IUser> result = new List<IUser>();
            result.AddRange(base.GetAllUsers().Select(x => new BasicUser() { Age = x.Age, FullName = x.FullName, PhoneNumber = x.PhoneNumber, Uid = x.Uid, UserName = x.UserName }));
            return result;
        }

        public BasicUser() : base()
        {

        }

        public override IUser GetUser(string Uid)
        {
            return Users.Find(item => item.Uid == Uid);
        }

        public override void AddUser(IUser user)
        {
            Users.Add(user);
        }

        public override List<Property> GetProperties()
        {
            return base.GetProperties();
        }

        public override IListViewer<IUser> UsersPageGet(IUser helperObj, string Filter, int ItemPerPage, int PageNumber)
        {
            return base.UsersPageGet(helperObj, Filter, ItemPerPage, PageNumber);
        }
    }
}
