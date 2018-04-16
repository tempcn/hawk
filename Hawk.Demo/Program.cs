using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hawk.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var role = new List<Role>();
            role.Add(new Role()
            {
                RoleId=10001,
                RoleName="qitong",
                IsAdmin=0,
                
            });
        }
    }

    /*
     SELECT a.RoleId,a.MenuId,a.Power,a.Privilege,
b.RoleName,b.IsAdmin,b.Remark as RoleRemark,
c.`Name`,c.DisplayName,c.ParentId,c.Url,c.Sort,c.Icon,c.Visible,c.Permission,c.Remark as MenuRemark 
FROM role_meun a LEFT JOIN role b ON a.RoleId=b.Id
LEFT JOIN menu c on a.MenuId=c.Id WHERE RoleId IN
(SELECT RoleId FROM user_role WHERE UserId=3);
         */

    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public byte IsAdmin { get; set; }
        public IEnumerable<Menu> Menu { get; set; }
        public IEnumerable<Permission> Permission { get; set; }
    }

    public class Menu
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public int Sort { get; set; }
        public string Icon { get; set; }
        public byte Visible { get; set; }
    }


    public enum Permission
    {
        Not = 0,
        Add = 1,
        Delete = 2,
        Update = 4,
        Select = 8,
        Print = 16,
        All = 1024
    }
}
