using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAO
{
    public class ProductCategoriesDAO
    {
        UniBootDbContext db = null;
        public ProductCategoriesDAO()
        {
            db = new UniBootDbContext();
        }
        public List<ProductCategory> ListAllProductCategory()
        {
            return db.ProductCategories.Where(x => x.Status == true).OrderBy(x => x.DisplayOrder).ToList();
        }

        public ProductCategory ProductCategoriesViewDetail(long? id)
        {
            return db.ProductCategories.Find(id);
        }
    }
}
