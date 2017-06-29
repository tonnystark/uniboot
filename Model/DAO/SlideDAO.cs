using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAO
{
    public class SlideDAO
    {
        UniBootDbContext db = null;
         public SlideDAO()
        {
            db = new UniBootDbContext();
        }
        public List<Slide> ListAllSlide()
        {
            return db.Slides.Where(x => x.Status == true).OrderBy(x => x.DisplayOrder).ToList();
        }
    }
}
