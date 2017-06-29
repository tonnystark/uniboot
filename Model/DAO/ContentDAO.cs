using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using PagedList;
using System.Web;

namespace Model.DAO
{
    public class ContentDAO
    {
        UniBootDbContext db = null;
        public static string USER_SESSION = "USER_SESSION";
        public ContentDAO()
        {
            db = new UniBootDbContext();
        }
        /// <summary>
        /// trả về một content
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Content GetContentByID(long id)
        {
            return db.Contents.Find(id);
        }

        public long CreateContent(Content content)
        {
            // xử lí alias
            if (!string.IsNullOrEmpty(content.MetaTitle))
            {
                content.MetaTitle = StringHelper.convertToUnSign(content.Name);
            }

            content.CreatedDate = DateTime.Now;
            content.ViewCount = 0;

            db.Contents.Add(content);
            db.SaveChanges();

            // Xử lí tag
            if (!string.IsNullOrEmpty(content.Tags))
            {
                // các tag cách nhau bởi dấu ','
                string[] tags = content.Tags.Split(',');
                foreach (var tag in tags)
                {
                    // id chính là Tag đã lọc dấu
                    var tagID = StringHelper.convertToUnSign(tag);
                    // kiểm tra tag có tồn tại chưa
                    var existTag = CheckTag(tagID);
                    // chưa có => thêm
                    if (!existTag)
                    {
                        InsertTag(tagID, tag);
                    }

                    // thêm vào content tag
                    InsertContentTag(content.ID, tagID);
                }
            }
            return content.ID;
        }

        /// <summary>
        ///  Kiểm tra tag đã tồn tại chưa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckTag(string id)
        {
            return db.Tags.Count(x => x.ID == id) > 0;
        }

        /// <summary>
        /// Thêm vào 1 tag
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public void InsertTag(string id, string name)
        {
            var tag = new Tag();
            tag.ID = id;
            tag.Name = name;

            db.Tags.Add(tag);
            db.SaveChanges();
        }

        /// <summary>
        ///  Thêm 1 contentTag
        /// </summary>
        /// <param name="contentID"></param>
        /// <param name="tagID"></param>
        public void InsertContentTag(long contentID, string tagID)
        {
            var contentTag = new ContentTag();
            contentTag.TagID = tagID;
            contentTag.ContentID = contentID;

            db.ContentTags.Add(contentTag);
            db.SaveChanges();
        }

        public bool ChangeStatus(long id)
        {
            var content = db.Contents.Find(id);
            // nếu stt đang true thì => false

            content.Status = !content.Status;
            // lưu lại thay đổi
            db.SaveChanges();

            return content.Status;
        }

        /// <summary>
        /// Lấy ra danh sách content
        /// </summary>
        /// <param name="strSearch"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Content> GetAllContentPagging(string strSearch, int page, int pageSize)
        {
            IQueryable<Content> model = db.Contents;

            if (!string.IsNullOrEmpty(strSearch))
            {
                model = model.Where(x => x.Name.Contains(strSearch) || x.Name.Contains(strSearch));
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        /// <summary>
        /// Lấy ra ds content cho trang client
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Content> GetAllContent(int page, int pageSize)
        {
            IQueryable<Content> model = db.Contents.Where(x => x.Status == true);

            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public long EditContent(Content content)
        {
            var findContent = db.Contents.Find(content.ID);
            try
            {
                // xử lí alias
                if (!string.IsNullOrEmpty(content.MetaTitle))
                {
                    content.MetaTitle = StringHelper.convertToUnSign(content.Name);
                }

                findContent.Name = content.Name;
                findContent.Description = content.Description;
                findContent.Detail = content.Detail;
                findContent.CreatedDate = DateTime.Now;
                findContent.MetaTitle = content.MetaTitle;
                findContent.Image = content.Image;
                findContent.CategoryID = content.CategoryID;
                findContent.Warranty = content.Warranty;
                findContent.MetaKeywords = content.MetaKeywords;
                findContent.MetaDescriptions = content.MetaDescriptions;
                findContent.Status = content.Status;
                findContent.Tags = content.Tags;
                findContent.Language = content.Language;

            }
            catch
            {
                return 0;
            }

            db.SaveChanges();

            // Xử lí tag
            if (!string.IsNullOrEmpty(findContent.Tags))
            {
                RemoveContentTag(findContent.ID);

                // các tag cách nhau bởi dấu ','
                string[] tags = findContent.Tags.Split(',');
                foreach (var tag in tags)
                {
                    // id chính là Tag đã lọc dấu
                    var tagID = StringHelper.convertToUnSign(tag);
                    // kiểm tra tag có tồn tại chưa
                    var existTag = CheckTag(tagID);
                    // chưa có => thêm
                    if (!existTag)
                    {
                        InsertTag(tagID, tag);
                    }

                    // thêm vào content tag
                    InsertContentTag(findContent.ID, tagID);
                }
            }
            return findContent.ID;
        }

        /// <summary>
        /// Xóa bỏ các tag
        /// </summary>
        /// <param name="contentID"></param>
        public void RemoveContentTag(long contentID)
        {
            db.ContentTags.RemoveRange(db.ContentTags.Where(x => x.ContentID == contentID));
            db.SaveChanges();
        }

        /// <summary>
        /// Lấy ra danh sách Tag theo content
        /// </summary>
        /// <param name="contentID"></param>
        /// <returns></returns>
        public List<Tag> GetTagByContent(long contentID)
        {
            var model = (from c in db.ContentTags
                         join t in db.Tags on c.TagID equals t.ID
                         where c.ContentID == contentID
                         select new
                         {
                             ID = t.ID,
                             Name = t.Name
                         }).AsEnumerable().Select(x => new Tag
                         {
                             ID = x.ID,
                             Name = x.Name
                         });

            return model.ToList();
        }

        /// <summary>
        /// Lấy ra tất cả content theo các thẻ Tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Content> GetAllContentByTag(string tag, int page, int pageSize)
        {
            var model = (from a in db.Contents
                         join b in db.ContentTags
                         on a.ID equals b.ContentID
                         where b.TagID == tag
                         select new
                         {
                             Name = a.Name,                         
                             MetaTitle = a.MetaTitle,
                             Image = a.Image,
                             Description = a.Description,
                             CreatedDate = a.CreatedDate,
                             CreatedBy = a.CreatedBy,
                             ID = a.ID
                         }).AsEnumerable().Select(x => new Content()
                         {
                             Name = x.Name,                         
                             MetaTitle = x.MetaTitle,
                             Image = x.Image,
                             Description = x.Description,
                             CreatedDate = x.CreatedDate,
                             CreatedBy = x.CreatedBy,
                             ID = x.ID
                         });
          
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public Tag GetTagByID(string id)
        {
            return db.Tags.Find(id);
        }


    }
}
