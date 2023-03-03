using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Tapudb.Models;

namespace Tapudb.Controllers
{
    public class KullaniciController : Controller
    {
        public IActionResult Index()
        {

            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                connection.Open();
                var kullanicilar = connection.Query<KullaniciViewModel>("select * from viewkullanicilar").ToList();
                return View(kullanicilar);

            }
        }
        [HttpGet]
        public ActionResult Yeni()
        {

            return View("Yeni", new KullaniciViewModel());
        } 
        public ActionResult FindByName(string text)
        {
            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                connection.Open();
                var kullanicilar = connection.Query<KullaniciViewModel>("select * from kullanicilar where Ad like (@text)", new {text=text}).ToList();
                return View("Index", kullanicilar);


            }


        }
        public ActionResult FindBySurname(string text)
        {
            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                connection.Open();
                var kullanicilar = connection.Query<KullaniciViewModel>("select * from soyadagoreara(@text)", new {text=text});
                var kullanicilar2 = connection.Query<KullaniciViewModel>("select * from kullanicilar where Soyad = @text", new {text=kullanicilar.First().Soyad});
                return View("Index", kullanicilar2);


            }


        }
        public ActionResult FindByMoney(string text)
        {
            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                connection.Open();
                var kullanicilar = connection.Query<KullaniciViewModel>("select * from parayagoreara(@text)", new { text = text });
                var kullanicilar2 = connection.Query<KullaniciViewModel>("select * from kullanicilar where para = @text", new { text = kullanicilar.First().para });
                return View("Index", kullanicilar2);


            }


        }
        
        public ActionResult Guncelle(int id)
        {

            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                var kullanici = connection.Query<KullaniciViewModel>("select * from kullanicilar where Id=@id", new {id=id}).Single();
                return View("Guncelle",kullanici);
            }
            
        }
        [HttpPost]
        public ActionResult Kaydet(KullaniciViewModel kullaici)
        {
            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                connection.Open();
                connection.Execute("insert into kullanicilar (ad,soyad,para) values ((@Ad),(@Soyad),(@para))", new KullaniciViewModel { Ad = kullaici.Ad, Soyad = kullaici.Soyad, para = kullaici.para });

                return RedirectToAction("Index");

            }
        }
        [HttpPost]
        public ActionResult Guncelle(KullaniciViewModel kullanici)
        {

            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                connection.Execute("update kullanicilar set Ad=@Ad,Soyad=@Soyad,para=@para where id=@id", new KullaniciViewModel { Ad = kullanici.Ad, Soyad = kullanici.Soyad, Id=kullanici.Id,para=kullanici.para });
            }
            return RedirectToAction("Index");
        }
        public ActionResult Sil(int id)
        {

            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
               connection.Execute("delete from kullanicilar where Id=@id", new { id = id });
                return RedirectToAction("Index");
            }

        }

    }
}
