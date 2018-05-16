using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public int Power { get; set; }
        public string Privilege { get; set; }
        public string RoleName { get; set; }
        public byte IsAdmin { get; set; }
        public string RoleRemark { get; set; }
        public string MenuName { get; set; }
        public string DisplayName { get; set; }
        public int ParentId { get; set; }
        public string Url { get; set; }
        public int Sort { get; set; }
        public string Icon { get; set; }
        public byte Visible { get; set; }
        public string Permission { get; set; }
        public string MenuRemark { get; set; }
    }

    public class Privilege
    {
        public string MenuName { get; set; }
        public int Power { get; set; }
        public IEnumerable<Operate> Permission { get; set; }
    }

    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public byte IsAdmin { get; set; }
        public int Created { get; set; }
        public DateTime Updated { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }
        public string Remark { get; set; }
    }

    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int ParentId { get; set; }
        public string Url { get; set; }
        public int Sort { get; set; }
        public string Icon { get; set; }
        public byte Visible { get; set; }
        public string Permission { get; set; }
        public string Remark { get; set; }
    }


    public enum Operate
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
