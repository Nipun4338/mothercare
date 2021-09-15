using mothercare.Data;
using mothercare.Helper;
using mothercare.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
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
                MailMessage msg = new MailMessage();
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                string link = "https://localhost:44301/Home/Verify?email="+user.EmailId+"&hash=" + user.EmailHash;
                try
                {
                    msg.Subject = "Regarding Registration";
                    msg.Body = "<h1 style=" + "\"background-image: linear-gradient(to top, rgba(255,192,203,0.5), rgba(255,192,203,1));text-align: center;font - family: 'Montserrat'; font - size: 22px;\"> Mother Care </h1>"+
                                "<p> Greetings! </p>"+
                                "<p> Please Confirm Your Account! </p>"+
                                   "<a href=\"" + link+ "\">Click Here!</a>";
                    msg.From = new MailAddress("mother.care.aust@gmail.com", "Mother Care");
                    msg.To.Add(user.EmailId);
                    msg.IsBodyHtml = true;
                    client.Host = "smtp.gmail.com";
                    System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("mother.care.aust@gmail.com", "wluewaekokkgffui");
                    client.Port = int.Parse("587");
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = basicauthenticationinfo;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Send(msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                isSuccess = false;
            }

            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Verify(string url)
        {
            dbmothercareEntities db = new dbmothercareEntities();
            string email = Request.QueryString["email"];
            string hash= Request.QueryString["hash"];
            var dataItem = db.Tbl_Members.Where(x => x.EmailId == email && x.EmailHash == hash).SingleOrDefault();
            if(dataItem!=null)
            {
                dataItem.IsActive = true;
                db.SaveChanges();
                ViewBag.Message = "Your account has been confirmed! Please Login.";
                ViewBag.Status = 1;
            }
            else
            {
                ViewBag.Message = "Anything wrong?";
                ViewBag.Status = 0;
            }
            return View();
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