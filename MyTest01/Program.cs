using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MyTest01.Models;

namespace MyTest01
{
    class Program
    {

        static void Main(string[] args)
        {

            //1. 原始失敗案例: 
            //Violation of PRIMARY KEY constraint 'PK_Roles'. Cannot insert duplicate key in object 'dbo.Roles'. The duplicate key value is (Role01).
            try
            {
                Console.WriteLine("========== 原始失敗案例 ==============");
                Execute_NG();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            ////2. 成功案例: (先決條件需要給不同的 PK 值)
            ////如果 3 個 table 都給新的 PK 值, 就會正常; 但系統需求上, 有可能因為只是修改部份內容, PK 會相同.
            //try
            //{
            //    Console.WriteLine("========== 成功案例: (先決條件需要給不同的 PK 值) ==============");
            //    Execute_OK_With_New_PK();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}

            //3. 成功案例: (使用 TransactionScope 或 BeginTransaction)
            // https://stackoverflow.com/questions/7335582/dbcontext-savechanges-order-of-statement-execution/7335895#7335895
            // https://jeffprogrammer.wordpress.com/2016/12/06/entity-framework-with-transactionscope/
            try
            {
                Console.WriteLine("========== 成功案例: (使用 TransactionScope 或 BeginTransaction) ==============");
                Execute_OK_With_TransactionScope();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Execute_NG()
        {
            using (var db = new ManyToManyDbEntities())
            {

                //讀取資料
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

        static void Execute_OK_With_New_PK()
        {

            using (var db = new ManyToManyDbEntities())
            {

                //讀取資料
                var users = db.Users.ToList();
                var roles = db.Roles.ToList();
                var userRoles = db.UserRoles.ToList();
                //3 個 table 都給新的 PK 值
                var usersAdd = users.Select(x => new User() { UserId = x.UserId + "N", UserName = x.UserName }).ToList();
                var rolesAdd = roles.Select(x => new Role() { RoleId = x.RoleId + "N", RoleName = x.RoleName }).ToList();
                var userRolesAdd = userRoles.Select(x => new UserRole() { UserId = x.UserId + "N", RoleId = x.RoleId + "N" }).ToList();

                //刪除資料
                db.UserRoles.RemoveRange(userRoles);
                db.Roles.RemoveRange(roles);
                db.Users.RemoveRange(users);
                //db.Database.Log = Console.WriteLine;
                //db.SaveChanges();

                Console.WriteLine("Press any key to continue (3)...");
                Console.ReadLine();

                //加入資料
                db.Users.AddRange(usersAdd);
                db.Roles.AddRange(rolesAdd);
                db.UserRoles.AddRange(userRolesAdd);
                db.Database.Log = Console.WriteLine;
                db.SaveChanges();

                Console.WriteLine("Press any key to continue (4)...");
                Console.ReadLine();

            }

        }

        static void Execute_OK_With_TransactionScope()
        {
            using (var db = new ManyToManyDbEntities())
            {

                //讀取資料
                var users = db.Users.ToList();
                var roles = db.Roles.ToList();
                var userRoles = db.UserRoles.ToList();
                var userRolesAdd = userRoles.Select(x => new UserRole() { UserId = x.UserId, RoleId = x.RoleId }).ToList();

                using (TransactionScope ts = new TransactionScope())
                {
                    //刪除資料
                    db.UserRoles.RemoveRange(userRoles);
                    db.Roles.RemoveRange(roles);
                    db.Users.RemoveRange(users);
                    db.Database.Log = Console.WriteLine;
                    db.SaveChanges();

                    Console.WriteLine("Press any key to continue (5)...");
                    Console.ReadLine();

                    //加入資料
                    db.Users.AddRange(users);
                    db.Roles.AddRange(roles);
                    db.UserRoles.AddRange(userRolesAdd);
                    db.Database.Log = Console.WriteLine;
                    db.SaveChanges();

                    Console.WriteLine("Press any key to continue (6)...");
                    Console.ReadLine();

                    ts.Complete();
                }
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
