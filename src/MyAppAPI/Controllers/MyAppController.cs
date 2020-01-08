using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using MyAppLogic;
using MappingLibraries;
using System.Text.Json;
using System.Collections;

namespace MyAppAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MyAppController : ControllerBase
    {
        int ItemPerPage { get; set; } = 10;

        [HttpGet]
        [ActionName("GetUsers")]
        public object GetUsers(int PageNumber, string Filter = "")
        {
            IUser helperObj = Main.CreateUser();
            var iResult = helperObj.UsersPageGet(helperObj, Filter, ItemPerPage, PageNumber);

            //var s =  JsonSerializer.Serialize(result);
            //var s = JsonSerializer.Serialize(result, Main.GetViewerType(""));
            


            Type objType = helperObj.GetType();

            var innerlistType = typeof(List<>).MakeGenericType(new[] { objType });
            if (!(innerlistType.FullName.Equals(iResult.Items.GetType().FullName) || (iResult.Items.Count > 0 && iResult.Items[0].GetType().FullName.Equals(typeof(IUser).FullName))))
            {
                var innerList = (IList)Activator.CreateInstance(innerlistType);
                foreach (var item in iResult.Items)
                {
                    innerList.Add(Convert.ChangeType(item, objType));
                }
                var listType = typeof(IListViewer<>).MakeGenericType(new[] { objType });
                var result = Activator.CreateInstance(listType, innerList, iResult.Properties, iResult.ItemsCount, iResult.ItemsPerPage, iResult.CurrentPage);

                return result;
            }
            else
            {
                return iResult;
            }


            //var innerList = iResult.Items.Select(x => ).ToList();

            //return result.Items.Select(i => Convert.ChangeType(i, helperObj.GetType()));

            //var obj2  = Convert.ChangeType(result, Main.GetViewerType(""));

           // return obj2;
            //return null;
           // return new JsonResult(result, new JsonSerializerOptions() {  });
        }

    }
}