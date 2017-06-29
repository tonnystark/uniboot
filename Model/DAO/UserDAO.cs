using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using PagedList;
using Common;

namespace Model.DAO
{
    public class UserDAO
    {
        // khai báo dbContext
        UniBootDbContext db = null;
        public UserDAO()
        {
            db = new UniBootDbContext();
        }

        /// <summary>
        /// Thêm vào 1 user
        /// </summary>
        /// <param name="user entity"></param>
        /// <returns>trả về id của user vừa thêm</returns>
        public long Insert(User entity)
        {
            db.Users.Add(entity);
            db.SaveChanges();
            // trả ra Id
            return entity.ID;
        }

        public long InsertForFacebook(User entity)
        {
            var user = db.Users.SingleOrDefault(x => x.UserName == entity.UserName);
            if (user != null)
            {
                return user.ID;
            }
            else
            {
                db.Users.Add(entity);
                db.SaveChanges();
                return entity.ID;
            }
        }

        /// <summary>
        /// Trả về danh sách trang các user có hiện tại
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> GetAllUserPagging(string strSearch, int page, int pageSize)
        {
            var model = db.Users.OrderByDescending(x => x.CreatedDate);

            if (!string.IsNullOrEmpty(strSearch))
            {
                model = model.Where(x => x.UserName.Contains(strSearch) || x.Name.Contains(strSearch)).OrderByDescending(x => x.CreatedDate);
            }
            return model.ToPagedList(page, pageSize);
        }

        //Thông tin login
        public enum LoginInfo
        {
            // đăng nhập đúng
            True = 1,
            // sai pass
            False = -1,
            // sai account
            InvalidUser = 0,
            // tài khoản bị block
            BlockedUser = -2,
            // không có quyền
            Permission_Denied = -3
        };

        /// <summary>
        /// lấy ra các quyền theo groupID dựa trên userName truyền vào
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<string> GetListCredential(string userName)
        {
            // lấy ra user
            var user = db.Users.Single(x => x.UserName == userName);
                        
            var data = (from a in db.Credentials
                       join b in db.UserGroups
                       on a.UserGroupID equals b.ID
                       join c in db.Roles on a.RoleID equals c.ID
                       where b.ID == user.GroupID
                       select new
                       {
                         RoleID =  a.RoleID
                       }).AsEnumerable().Select(x => new Credential {
                           RoleID = x.RoleID
                       });
            return data.Select(x => x.RoleID).ToList();
        }

        /// <summary>
        /// Kiểm tra user login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public int Login(string userName, string passWord, bool isAdmin = false)
        {
            var result = db.Users.SingleOrDefault(x => x.UserName == userName);
            // sai tk
            if (result == null)
                return (int)LoginInfo.InvalidUser;            
            // admin login
            if (isAdmin == true)
            {
                // nếu là tài khoản admin hay mod
                if (result.GroupID == CommonConstant.ADMIN_GROUP || result.GroupID == CommonConstant.MOD_GROUP)
                {
                    // bị khóa
                    if (result.Status == false)
                        return (int)LoginInfo.BlockedUser;
                    // login đúng pass
                    if (result.Password == passWord)
                        return (int)LoginInfo.True;
                    return (int)LoginInfo.False;
                }
                else // không có quyền đăng nhập
                    return (int)LoginInfo.Permission_Denied;
            }
            else // user bìh thường login
            {
                // bị khóa
                if (result.Status == false)
                    return (int)LoginInfo.BlockedUser;
                // login đúng pass
                if (result.Password == passWord)
                    return (int)LoginInfo.True;
                return (int)LoginInfo.False;
            }
        }

        /// <summary>
        /// Tìm ra User trong dbContext ứng với userName tương ứng
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>một User</returns>
        public User FindUserByUserName(string userName)
        {
            return db.Users.SingleOrDefault(x => x.UserName == userName);
        }

        /// <summary>
        /// Tìm user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User FindUserById(long id)
        {
            return db.Users.Find(id);
        }

        /// <summary>
        /// Cập nhật thông tin User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateUser(User user)
        {
            var findUser = db.Users.Find(user.ID);
            try
            {
                // pass phải khác rỗng
                if (!String.IsNullOrEmpty(user.Password))
                    findUser.Password = user.Password;

                findUser.Name = user.Name;
                findUser.Email = user.Email;
                findUser.ModifiedBy = user.ModifiedBy;
                findUser.ModifiedDate = DateTime.Now;
                findUser.Address = user.Address;
                findUser.Phone = user.Phone;
                findUser.Status = user.Status;
                findUser.GroupID = user.GroupID;
            }
            catch (Exception ex)
            {
                return false;
            }
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// Xóa User tại Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteUser(long id)
        {
            try
            {
                var user = db.Users.Find(id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Đổi lại status của User hiện tại
        /// Nếu Status hiện tại là true => false
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ChangeStatus(long id)
        {
            var user = db.Users.Find(id);
            // nếu stt đang true thì => false

            user.Status = !user.Status;
            // lưu lại thay đổi
            db.SaveChanges();

            return user.Status;
        }

        /// <summary>
        /// Kiểm tra userName, Nếu > 0 return true: đã có, ngược lại: chưa có
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool CheckUserName(string userName)
        {
            return db.Users.Count(x => x.UserName == userName) > 0;
        }
        public bool CheckEmail(string email)
        {
            return db.Users.Count(x => x.Email == email) > 0;
        }

        public List<UserGroup> GetGroupUser()
        {
            return db.UserGroups.ToList();
        }
    }
}
