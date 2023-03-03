using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Tapudb.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tapudb.Controllers
{
    public class TapuController : Controller
    {
        public IActionResult Index()
        {

            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                connection.Open();
                var tapular = connection.Query<TapuViewModel>("select * from tapular");
                //     var tapular = connection.Query<TapuViewModel>("select * from tapular left join kullanicilar on kullanicilar.Id=tapular.Id").ToList();
                foreach (var item in tapular)
                {
                    var kullanici = connection.Query<KullaniciViewModel>("select * from kullanicilar where Id=(@id)", new { id = item.sahipId });
                    if (tapular.Any() && kullanici.Any())
                    {
                        item.Kullanici = kullanici.FirstOrDefault();
                    }
                }
 
                return View(tapular);

            }
        }

        public ActionResult FindByTapu(string text)
        {
            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                connection.Open();
                var tapu = connection.Query<TapuViewModel>("select * from adresegoreara(@text)", new { text = text });
                var tapu2 = connection.Query<TapuViewModel>("select * from tapular where Adres = @text", new { text = tapu.First().Adres });
                
                foreach (var item in tapu2)
                {
                    var kullanici = connection.Query<KullaniciViewModel>("select * from kullanicilar where Id=(@id)", new { id = item.sahipId });
                    item.Kullanici = kullanici.First();
                }
                return View("Index", tapu2);


            }


        }

        [HttpGet]
        public ActionResult Yeni()
        {
            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                connection.Open();
                var kullanicilar = connection.Query<KullaniciViewModel>("select * from viewkullanicilar").ToList();
                
                return View("Yeni", new TapuListViewModel() { Kullanicilar=kullanicilar,Tapu=new TapuViewModel()});
            }
            
        }
        


        //}
        [HttpPost]
        public ActionResult Kaydet(TapuViewModel tapu)
        {
            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                connection.Open();
                var sayi1 = connection.Query<TapuViewModel>("select count(*) as sayi from tapular");
                connection.Execute("insert into tapular (adres,kayittarihi,sahipid) values ((@Adres),(@KayitTarihi),(@sahipid))", new TapuViewModel { Adres = tapu.Adres, KayitTarihi = tapu.KayitTarihi,sahipId=tapu.sahipId});
                var model=connection.Query<TapuViewModel>("select * from tapular").Last();
                connection.Execute("insert into sahiplik (tapuid,kullaniciid,alistarihi) values ((@tapuid),(@kullaniciid),(@alistarihi))", new { tapuid = model.Id, kullaniciid =model.sahipId , alistarihi = DateTime.Now });
                var sayi2 = connection.Query<TapuViewModel>("select count(*) as sayi from tapular");
                if(sayi1.First().sayi==sayi2.First().sayi)
                {
                    return View("Eklenemedi");
                }


                return RedirectToAction("Index");

            }
        }
        public ActionResult Guncelle(int id)
        {

            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                var tapu = connection.Query<TapuViewModel>("select * from tapular where Id=@id", new { id = id }).Single();
                var kullanicilar = connection.Query<KullaniciViewModel>("select * from kullanicilar").ToList();
                var tapulistviewmodel = new TapuListViewModel { 
                    Tapu = new TapuViewModel { Adres = tapu.Adres, KayitTarihi = tapu.KayitTarihi, sahipId = tapu.sahipId,Id=tapu.Id },
                    Kullanicilar = kullanicilar };

                return View("Guncelle", tapulistviewmodel);
            }

        }
        [HttpPost]
        public ActionResult Guncelle(TapuViewModel tapu)
        {

            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                
                connection.Execute("update tapular set adres=@adres,kayittarihi=(@kayittarihi),sahipid=@sahipid where id=@id",new{adres=tapu.Adres,kayittarihi=tapu.KayitTarihi,sahipid=tapu.sahipId,id=tapu.Id}) ;
            }
            return RedirectToAction("Index");
        }
        public ActionResult Sil(int id)
        {

            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                connection.Execute("delete from tapular where Id=@id", new { id = id });
                return RedirectToAction("Index");
            }

        }
    }
}
