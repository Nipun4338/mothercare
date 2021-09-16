using mothercare.Data;
using mothercare.Repository;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mothercare.Models.Home
{
    public class HomeIndexViewModel
    {
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        dbmothercareEntities context = new dbmothercareEntities();

        public IPagedList<Tbl_Product> ListOfProducts { get; set; }
        public List<Tbl_Slider> slider { get; set; }

        public HomeIndexViewModel CreateModel(string search, int pageSize, int? page)
        {
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@search",search??(object)DBNull.Value)
            };
            IPagedList<Tbl_Product> data = context.Database.SqlQuery<Tbl_Product>("GetBySearch @search", param).ToList().ToPagedList(page ?? 1, pageSize);
            List<Tbl_Slider> data2 = context.Tbl_Slider.Where(x => x.IsDelete == false && x.IsActive==true).ToList();
            return new HomeIndexViewModel
            {
                ListOfProducts = data,
                slider=data2
            };
        }
    }
}