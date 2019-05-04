using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MyTest01.Models;

namespace MyTest01
{
    class Program
    {

        static void Main(string[] args)
        {
            using (var db = new ManyToManyDbEntities())
            {

                var users = db.Users.ToList();
                var roles = db.Roles.ToList();
                var userRoles = db.UserRoles.ToList();
                var userRolesAdd = userRoles.Select(x => new UserRole() { UserId = x.UserId, RoleId = x.RoleId }).ToList();

                //刪除資料
                db.UserRoles.RemoveRange(userRoles);
                db.Roles.RemoveRange(roles);
                db.Users.RemoveRange(users);
                //db.Database.Log = Console.WriteLine;
                //db.SaveChanges();

                Console.WriteLine("Press any key to continue (1)...");
                Console.ReadLine();

                //加入資料
                db.Users.AddRange(users);
                db.Roles.AddRange(roles);
                db.UserRoles.AddRange(userRolesAdd);
                db.Database.Log = Console.WriteLine;
                db.SaveChanges();

                Console.WriteLine("Press any key to continue (2)...");
                Console.ReadLine();

            }
        }

        static void DisplayAll()
        {
            using (var db = new ManyToManyDbEntities())
            {

                // db.Database.Log = Console.WriteLine;

                foreach (var user in db.Users)
                {
                    Console.WriteLine($"Id:{user.UserId} Name:{user.UserName}");
                }

                foreach (var role in db.Roles)
                {
                    Console.WriteLine($"Id:{role.RoleId} Name:{role.RoleName}");
                }

                foreach (var item in db.UserRoles)
                {
                    Console.WriteLine($"Id:{item.Id} UserId:{item.UserId} RoleId:{item.RoleId}");
                }

                Console.WriteLine("Press any key to continue ...");
                Console.ReadLine();

                //var users = new List<User>()
                //{
                //    new User() { UserId = "User01", UserName="JASPER"},
                //    new User() { UserId = "User02", UserName = "JUDY"}
                //};

                //var roles = new List<Role>()
                //{
                //    new Role()  { RoleId = "Role01", RoleName = "Project Manager"},
                //    new Role()  { RoleId = "Role02", RoleName = "Developer"},
                //    new Role()  { RoleId = "Role03", RoleName = "DBA"},
                //};

                //var userRoles = new List<UserRole>()
                //{
                //    new UserRole() { UserId = "User01", RoleId = "Role02"},
                //    new UserRole() { UserId = "User01", RoleId = "Role03"},
                //    new UserRole() { UserId = "User02", RoleId = "Role01"},
                //    new UserRole() { UserId = "User02", RoleId = "Role02"},
                //};

            }
        }
    }
}
