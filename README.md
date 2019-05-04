# MyTest01

## 情境:
3 個 Table ( Users / Roles / UserRoles ), Users 與 Roles 為多對多的關係. 參考如下的 <SQL.1>  
發現 Entity Framework 似乎沒有依程式的 RemoveRange / AddRange 的順序執行.  
只要放在同一個 dbcontext, 只作 1 次 SaveChanges(), 就會出現問題: "Violation of PRIMARY KEY constraint 'PK_Roles'. Cannot insert duplicate key in object 'dbo.Roles'. The duplicate key value is (Role01).
The statement has been terminated. "  
 若把刪除/新增各作 1 次 SaveChange(), 就會正常.  

## 程式順序:
  
//取得資料  
var users = db.Users.ToList();  
var roles = db.Roles.ToList();  
var userRoles = db.UserRoles.ToList();  
var userRolesAdd = userRoles.Select(x => new UserRole() { UserId = x.UserId, RoleId = x.RoleId }).ToList();   
  
//刪除資料 (如果先把刪除的部份先 SaveChanges(), 亦即解除註解, 就可以正常運作; 但這樣佰變成是 2 筆 交易)  
db.UserRoles.RemoveRange(userRoles);  
db.Roles.RemoveRange(roles);  
db.Users.RemoveRange(users);  
//db.Database.Log = Console.WriteLine;  
//db.SaveChanges();  
  
//加入資料  
db.Users.AddRange(users);  
db.Roles.AddRange(roles);  
db.UserRoles.AddRange(userRolesAdd);  
db.Database.Log = Console.WriteLine;  
db.SaveChanges();  
 

## SQL 的 Log.

 開啟連接  
 開始交易  
DELETE [dbo].[UserRoles]  
WHERE ([Id] = @0)  
-- @0: '1' (Type = Int32)  
  
DELETE [dbo].[UserRoles]  
WHERE ([Id] = @0)  
-- @0: '2' (Type = Int32)  
  
DELETE [dbo].[UserRoles]  
WHERE ([Id] = @0)  
-- @0: '3' (Type = Int32)  
  
DELETE [dbo].[UserRoles]  
WHERE ([Id] = @0)  
-- @0: '4' (Type = Int32)  
  
  
  
INSERT [dbo].[Roles]([RoleId], [RoleName])  
VALUES (@0, @1)  
-- @0: 'Role01' (Type = AnsiString, Size = 10)  
-- @1: 'Project Manager' (Type = AnsiString, Size = 32)  
-- 於 2019/5/4 下午 02:48:15 +08:00  
 執行  
-- 於 5 毫秒後失敗，發生錯誤: Violation of PRIMARY KEY constraint 'PK_Roles'. Cannot insert duplicate key in object 'dbo.Roles'. The duplicate key value is (Role01).  
The statement has been terminated.  
  
關閉連接  
  
## <SQL.1>
-- =============================  
-- 說明:  
-- 使用者與角色為 多對多 的關係  
-- =============================  
  
DROP TABLE IF EXISTS UserRoles;  
DROP TABLE IF EXISTS Roles;  
DROP TABLE IF EXISTS Users;  
GO  
  
CREATE TABLE Users   
(	[UserId]	VARCHAR (10)	NOT NULL  
,	[UserName]	VARCHAR (32)  
  
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC)  
);  
  
CREATE TABLE Roles   
(	[RoleId]	VARCHAR (10)	NOT NULL  
,	[RoleName]	VARCHAR (32)  
  
  CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleId] ASC)  
);  
  
CREATE TABLE UserRoles   
(	[Id]		INT		IDENTITY(1, 1)	NOT NULL  
,	[UserId]	VARCHAR (10)	NOT NULL  
,	[RoleId]	VARCHAR (10)	NOT NULL  
  
  CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([Id] ASC)  
,	CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])  
,	CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId])  
);  
GO  

INSERT INTO Users VALUES ( 'User01', 'JASPER' );  
INSERT INTO Users VALUES ( 'User02', 'JUDY' );  

INSERT INTO Roles VALUES ( 'Role01', 'Project Manager' );  
INSERT INTO Roles VALUES ( 'Role02', 'Developer' );  
INSERT INTO Roles VALUES ( 'Role03', 'DBA' );  
  
INSERT INTO UserRoles ([UserId], [RoleId])  VALUES ( 'User01', 'Role02' );  
INSERT INTO UserRoles ([UserId], [RoleId])  VALUES ( 'User01', 'Role03' );  
INSERT INTO UserRoles ([UserId], [RoleId])  VALUES ( 'User02', 'Role01' );  
INSERT INTO UserRoles ([UserId], [RoleId])  VALUES ( 'User02', 'Role02' );  
  
GO  

