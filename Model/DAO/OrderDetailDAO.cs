using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAO
{
    public class OrderDetailDAO
    {
        UniBootDbContext db = null;
        public OrderDetailDAO()
        {
            db = new UniBootDbContext();
        }
        /// <summary>
        /// Thêm chi tiết đơn hàng thanh toán
        /// /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool Insert(OrderDetail detail)
        {
            try
            {
                db.OrderDetails.Add(detail);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
