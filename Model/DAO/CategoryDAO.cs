using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAO
{
    public class CategoryDAO
    {
        UniBootDbContext db = null;
        public CategoryDAO()
        {
            db = new UniBootDbContext();
        }
        public List<Category> ListAllCategory()
        {
            return db.Categories.Where(x => x.Status == true).ToList();
        }

        public ProductCategory ViewDetail(long id)
        {
            return db.ProductCategories.Find(id);
        }

        public long InsertCategory(Category category)
        {
            db.Categories.Add(category);
            db.SaveChanges();
            return category.ID;
        }

    }
}
