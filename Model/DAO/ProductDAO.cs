using Common;
using Model.EF;
using Model.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAO
{
    public class ProductDAO
    {
        UniBootDbContext db = null;
        public ProductDAO()
        {
            db = new UniBootDbContext();
        }
        /// <summary>
        /// Lấy ra ds Product bởi categoryId
        /// </summary>
        /// <param name="cateId"></param>
        /// <returns></returns>
        public List<ProductViewModel> lstByCategoryId(long cateId, ref int totalRecords, int pageIndex = 1, int pageSize = 2)
        {
            // Sẽ  take(lấy) 2 dòng trong bảng
            // Nếu pageIndex = 1, tức là ở vị trí 0 => Skip 0, take 2
            // Nếu  pageIndex = 2, tức là ở vị trí 1 => Skip 1 * 2 = 2, take 2./
            totalRecords = db.Products.Where(x => x.CategoryID == cateId).Count();

            var model = (from a in db.Products
                         join b in db.ProductCategories
                         on a.CategoryID equals b.ID
                         where a.CategoryID == cateId
                         select new
                         {
                             CateMetaTitle = b.MetaTitle,
                             CateName = b.Name,
                             CreatedDate = a.CreatedDate,
                             ID = a.ID,
                             Images = a.Image,
                             Name = a.Name,
                             MetaTitle = a.MetaTitle,
                             Price = a.Price
                         }).AsEnumerable().Select(b => new ProductViewModel
                         {
                             CateMetaTitle = b.MetaTitle,
                             CateName = b.Name,
                             CreatedDate = b.CreatedDate,
                             ID = b.ID,
                             Images = b.Images,
                             Name = b.Name,
                             MetaTitle = b.MetaTitle,
                             Price = b.Price
                         });


            return model.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        /// <summary>
        /// Lấy ra danh sách tất cả sản phẩm
        /// theo 1 số lượng 'top' nhất định
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public List<Product> ListNewProducts(int top)
        {
            return db.Products.Where(x => x.Status == true).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }


        public List<Product> ListFeaturedProduct(int top)
        {
            return db.Products.Where(x => x.Status == true && x.TopHot != null).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }

        public Product ProductDetail(long id)
        {
            return db.Products.Find(id);
        }

        public bool ChangeStatus(long id)
        {
            var product = db.Products.Find(id);
            // nếu stt đang true thì => false

            product.Status = !product.Status;
            // lưu lại thay đổi
            db.SaveChanges();

            return product.Status;
        }

        /// <summary>
        /// Những sản phẩm cùng Category và khác id hiện tại
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Product> ListRelatedProducts(long id)
        {
            var idProductCate = db.Products.Find(id);
            return db.Products.Where(x => x.ID != id && x.CategoryID == idProductCate.CategoryID).OrderByDescending(x => x.CreatedDate).ToList();
        }

        /// <summary>
        /// Trả về danh sách tên các Product theo "keyword"
        /// Sử dụng trong việc tìm kiếm
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public List<string> ListNameProduct(string keyWord)
        {
            return db.Products.Where(x => x.Name.Contains(keyWord)).Select(x => x.Name).ToList();
        }

        /// <summary>
        /// Tìm ra các sản phẩm theo keyword có phân trang
        /// Sẽ được gọi ở Controller
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="totalRecords"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<ProductViewModel> Search(string keyword, ref int totalRecords, int pageIndex = 1, int pageSize = 2)
        {
            // Sẽ  take(lấy) 2 dòng trong bảng
            // Nếu pageIndex = 1, tức là ở vị trí 0 => Skip 0, take 2
            // Nếu  pageIndex = 2, tức là ở vị trí 1 => Skip 1 * 2 = 2, take 2./
            totalRecords = db.Products.Where(x => x.Name == keyword).Count();

            var model = (from a in db.Products
                         join b in db.ProductCategories
                         on a.CategoryID equals b.ID
                         where a.Name.Contains(keyword)
                         select new
                         {
                             CateMetaTitle = b.MetaTitle,
                             CateName = b.Name,
                             CreatedDate = a.CreatedDate,
                             ID = a.ID,
                             Images = a.Image,
                             Name = a.Name,
                             MetaTitle = a.MetaTitle,
                             Price = a.Price
                         }).AsEnumerable().Select(b => new ProductViewModel
                         {
                             CateMetaTitle = b.MetaTitle,
                             CateName = b.Name,
                             CreatedDate = b.CreatedDate,
                             ID = b.ID,
                             Images = b.Images,
                             Name = b.Name,
                             MetaTitle = b.MetaTitle,
                             Price = b.Price
                         });

            return model.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        /// <summary>
        /// Lấy ds tất cả loại sản phẩm
        /// </summary>
        /// <returns></returns>
        public List<ProductCategory> GetProductCategory()
        {
            return new ProductCategoriesDAO().ListAllProductCategory();
        }

        /// <summary>
        /// Tạo mới 1 product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public long CreateProduct(Product product)
        {
            if (!String.IsNullOrEmpty(product.MetaTitle))
            {
                product.MetaTitle = StringHelper.convertToUnSign(product.MetaTitle);
            }
            product.ViewCount = 0;

            db.Products.Add(product);
            db.SaveChanges();
            return product.ID;
        }

        public IEnumerable<Product> GetAllProductByKeyword(string strSearch = null, int page = 1, int pageSize = 5)
        {
            IQueryable<Product> model = db.Products.Where(x => x.Status == true);
            if (!string.IsNullOrEmpty(strSearch))
            {
                model = model.Where(x => x.Name.Contains(strSearch) || x.MetaKeywords.Contains(strSearch));
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public bool DeleteProduct(long id)
        {
            try
            {
                var proc = db.Products.Find(id);
                db.Products.Remove(proc);
                db.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Lấy ra sản phẩm theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Product GetProducById(long id)
        {
            return db.Products.Find(id);
        }

        /// <summary>
        /// Edit một sản phẩm
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        public long EditProduct(Product proc)
        {
            var findProc = db.Products.Find(proc.ID);
            try
            {
                // xử lí alias
                if (!string.IsNullOrEmpty(proc.MetaTitle))
                {
                    proc.MetaTitle = StringHelper.convertToUnSign(proc.Name);
                }

                findProc.Name = proc.Name;
                findProc.Description = proc.Description;
                findProc.Detail = proc.Detail;
                findProc.CreatedDate = DateTime.Now;
                findProc.MetaTitle = proc.MetaTitle;
                findProc.Image = proc.Image;
                findProc.CategoryID = proc.CategoryID;
                findProc.Warranty = proc.Warranty;
                findProc.MetaKeywords = proc.MetaKeywords;
                findProc.MetaDescriptions = proc.MetaDescriptions;
                findProc.Status = proc.Status;
                findProc.Price = proc.Price;
                findProc.PromotionPrice = proc.PromotionPrice;

            }
            catch
            {
                return 0;
            }

            db.SaveChanges();

            return findProc.ID;
        }

        public List<Product> GetAllProduct()
        {
            return db.Products.Where(x => x.Status == true).ToList();
        }

        /// <summary>
        /// Thêm nhiều ảnh
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="images"></param>
        /// <returns></returns>
        public bool UpdateImages(long productID, string images)
        {
            try
            {
                var product = db.Products.Find(productID);
                product.MoreImages = images;
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
