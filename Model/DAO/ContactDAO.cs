using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAO
{
    public class ContactDAO
    {
        UniBootDbContext db = null;
        public ContactDAO()
        {
            db = new UniBootDbContext();
        }
        public Contact GetActiveContact()
        {
            return db.Contacts.Single(x => x.Status == true);
        }
        /// <summary>
        /// Thêm vào 1 feedBack, return ra ID
        /// </summary>
        /// <param name="fb"></param>
        /// <returns></returns>
        public int InsertFeedBack(Feedback fb)
        {
            db.Feedbacks.Add(fb);
            db.SaveChanges();
            return fb.ID;
        }
    }
}
