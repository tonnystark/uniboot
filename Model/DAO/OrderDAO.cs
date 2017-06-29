using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;

namespace Model.DAO
{
    public class OrderDAO
    {
        UniBootDbContext db = null;
        public OrderDAO()
        {
            db = new UniBootDbContext();
        }
        /// <summary>
        /// Thêm 1 phiếu  để thanh toán
        /// Trả về ID của order vừa thêm
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public long Insert(Order order)
        {
            db.Orders.Add(order);
            db.SaveChanges();

            return order.ID;
        }
    }
}
