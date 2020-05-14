using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Tesseract;

namespace OpticalCharacterRecognition.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Assets/img"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    using (var engine = new TesseractEngine(Server.MapPath(@"~/tessdata"), "eng", EngineMode.Default))
                    {
                        using (var image = new System.Drawing.Bitmap(file.InputStream))
                        {
                            using (var pix = PixConverter.ToPix(image))
                            {
                                using (var page = engine.Process(pix))
                                {
                                    ViewBag.Rec = page.GetText();
                                }
                            }
                        }
                    }
                    ViewBag.Message = "File uploaded successfully";
                    ViewBag.ImagePath = "/Assets/img/" + file.FileName;
                    ViewBag.FileName = file.FileName;
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    ViewBag.Image = null;
                    ViewBag.FileName = null;
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
                ViewBag.Image = null;
                ViewBag.FileName = null;
            }
            return View();
        }
    }
}