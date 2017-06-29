using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAO
{
    public class MenuDAO
    {
        UniBootDbContext db = null;
        public MenuDAO()
        {
            db = new UniBootDbContext();
        }

        /// <summary>
        /// trả về 1 menu
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public List<Menu> LstByTypeId(int typeID)
        {            
            // chọn ra kiểu menu, sắp xếp 
            return db.Menus.Where(x => x.TypeID == typeID && x.Status == true).OrderBy(x => x.DisplayOrder).ToList();
        }
    }
}
