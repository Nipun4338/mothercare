using mothercare.Data;
using mothercare.Helper;
using mothercare.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace mothercare.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string search, int? page)
        {
            HomeIndexViewModel model = new HomeIndexViewModel();
            return View(model.CreateModel(search, 4, page));
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult UserCreate()
        {
            return View();
        }

        public JsonResult CheckLogin(string username, string password)
        {
            dbmothercareEntities db = new dbmothercareEntities();
            string md5StringPassword = AppHelper.GetMd5Hash(password);
            var dataItem = db.Tbl_Members.Where(x => x.EmailId == username && x.Password == md5StringPassword).SingleOrDefault();
           
            bool isLogged = true;
            if (dataItem != null)
            { 
                Session["Username"] = dataItem.EmailId;
                var roleitem = db.Tbl_MemberRole.Where(y => y.memberId == dataItem.MemberId).SingleOrDefault();
                if (roleitem.RoleId==1)
                {
                    Session["Role"] = "Admin";
                }
                else
                {
                    Session["Role"] = "User";
                }
                isLogged = true;
            }
            else
            {
                isLogged = false;
            }
            return Json(isLogged, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveUser(Tbl_Members user)
        {
            dbmothercareEntities db = new dbmothercareEntities();
            bool isSuccess = true;
            try
            {
                var random = new Random();
                int randomnumber = random.Next();
                user.EmailHash= AppHelper.GetMd5Hash(randomnumber.ToString());
                user.IsActive = false;
                user.Password = AppHelper.GetMd5Hash(user.Password);
                db.Tbl_Members.Add(user);
                db.SaveChanges();
                Tbl_MemberRole role = new Tbl_MemberRole();
                role.memberId = user.MemberId;
                role.RoleId = 2;
                db.Tbl_MemberRole.Add(role);
                db.SaveChanges();
                SmtpClient smtpClient = new SmtpClient();

                smtpClient.Credentials = new System.Net.NetworkCredential("mother.care.aust@gmail.com", "wluewaekokkgffui");
                // smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;
                MailMessage mail = new MailMessage();

                //Setting From , To and CC
                mail.From = new MailAddress("mother.care.aust@gmail.com", "Mother Care");
                mail.Bcc.Add(new MailAddress(user.EmailId));
                mail.Body = "Greetings!" +
                    "To confirm your mothercare account, please click- ";

                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                isSuccess = false;
            }

            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Logout()
        {
            Session["Username"] = null;
            Session["Role"] = null;
            return RedirectToAction("Login");
        }
    }
}