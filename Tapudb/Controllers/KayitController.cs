using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Tapudb.Models;

namespace Tapudb.Controllers
{
    public class KayitController : Controller
    {
        public IActionResult Index()
        {
            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                var kayitlar = connection.Query<KayitViewModel>("select * from kayitlar");
                foreach (var item in kayitlar)
                {
                    var tapu = connection.Query<TapuViewModel>("select * from tapular where id=@id", new { id = item.tapuid });
                    var alankullanici = connection.Query<KullaniciViewModel>("select * from kullanicilar where id=@id", new { id = item.alankisi});
                    var verenkullanici = connection.Query<KullaniciViewModel>("select * from kullanicilar where id=@id", new { id = item.verenkisi });
                    if (kayitlar.Any() && tapu.Any()&&alankullanici.Any()&&verenkullanici.Any())
                    {

                        item.tapu = tapu.First();
                        item.Kullanici=alankullanici.First();
                        item.Kullanici2=verenkullanici.First();
                    }
                }
                return View(kayitlar);
            }
        }
        [HttpGet]
        public ActionResult Yeni()
        {
            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                connection.Open();
                var kullanicilar = connection.Query<KullaniciViewModel>("select * from viewkullanicilar").ToList();
                var tapular = connection.Query<TapuViewModel>("select * from tapular").ToList();

                return View("Yeni", new KayitListViewModel() {kullanicilar=kullanicilar,tapular=tapular,kayit=new KayitViewModel() });
            }

        }
        [HttpPost]
        public ActionResult Kaydet(KayitViewModel kayit)
        {
            int count = 0;
            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                connection.Open();
                var ucret = ((kayit.fiyat) * (0.05))/2;
                var alankisi = connection.Query<KayitViewModel>("select para from kullanicilar where id=@id", new { id = kayit.alankisi });
                var verenkisi = connection.Query<KayitViewModel>("select para from kullanicilar where id=@id", new { id = kayit.verenkisi });
                var model = connection.Query<KayitViewModel>("select tapuid from kayitlar");
                var alankisipara = -kayit.fiyat+ alankisi.First().para - ucret-10;
                var verenkisipara = kayit.fiyat + verenkisi.First().para - ucret-10;

                connection.Execute("update kullanicilar set para=@para where id=@id", new { para = alankisipara, id = kayit.alankisi });
                connection.Execute("update kullanicilar set para=@para where id=@id", new { para = verenkisipara, id = kayit.verenkisi });
                var sayi1 = connection.Query<KayitViewModel>("select count(*) as sayi from kayitlar");
                //foreach (var item in model)
                //{
                //    if(item.tapuid==kayit.tapuid)
                //    {
                //        count++;
                //    }
                //}
                //if(count==0)
                //{

                connection.Execute("insert into kayitlar (alankisi,verenkisi,fiyat,tapuid) values ((@alankisi),(@verenkisi),(@fiyat),(@tapuid))", new
                {
                    alankisi = kayit.alankisi,
                    verenkisi = kayit.verenkisi,
                    fiyat = kayit.fiyat,
                    tapuid = kayit.tapuid
                });
                //}

                var sayi2 = connection.Query<KayitViewModel>("select count(*) as sayi from kayitlar");
                if (sayi1.First().sayi == sayi2.First().sayi)
                {
                    return View("Eklenemedi");
                }
                else
                {

                    
                    connection.Execute("update sahiplik set tapuid=@tapuid,kullaniciid=@kullaniciid,alistarihi=@alistarihi where tapuid=@tapuid", new { tapuid = kayit.tapuid, kullaniciid = kayit.alankisi, alistarihi = DateTime.Now });
                  //  connection.Execute("update tapular set sahipid=@sahipid where id=@id", new { sahipid = kayit.alankisi,id=kayit.tapuid });

                }
                return RedirectToAction("Index");

            }
        }
        public ActionResult Guncelle(int id)
        {

            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                var kayit = connection.Query<KayitViewModel>("select * from kayitlar where Id=@id", new { id = id }).Single();
                var kullanicilar = connection.Query<KullaniciViewModel>("select * from kullanicilar").ToList();
                var tapular = connection.Query<TapuViewModel>("select * from tapular").ToList();
                var kayitlistviewmodel = new KayitListViewModel
                {
                    kayit = kayit,
                    kullanicilar = kullanicilar,
                    tapular = tapular
                };

                return View("Guncelle", kayitlistviewmodel);
            }

        }
        [HttpPost]
        public ActionResult Guncelle(KayitViewModel kayit)
        {

            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {

                connection.Execute("update kayitlar set alankisi=@alankisi,verenkisi=(@verenkisi),fiyat=@fiyat,tapuid=@tapuid where id=@id", new { alankisi = kayit.alankisi, verenkisi = kayit.verenkisi, fiyat = kayit.fiyat, tapuid = kayit.tapuid,id=kayit.Id });
              //  connection.Execute("update sahiplik set (tapuid=@tapuid,kullaniciid=@kullaniciid,alistarihi=@alistarihi) ((@tapuid),(@kullaniciid),(@alistarihi))", new { tapuid = kayit.tapuid, kullaniciid = kayit.alankisi, alistarihi = DateTime.Now });


            }
            return RedirectToAction("Index");
        }
        public ActionResult Sil(int id)
        {

            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
               var model= connection.Query<KayitViewModel>("select * from kayitlar where id=@id", new { id = id }).First();
                connection.Execute("delete from sahiplik where kullaniciid=@kullaniciid and tapuid=@tapuid", new {kullaniciid=model.alankisi,tapuid=model.tapuid});
                connection.Execute("delete from kayitlar where Id=@id", new { id = id });
                return RedirectToAction("Index");
           //     connection.Execute("delete from sahiplik");

            }

        } 
        public ActionResult Ticaret()
        {
            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                var model = connection.Query<KayitViewModel>("select alankisi from kayitlar UNION select verenkisi from kayitlar");
                foreach (var item in model)
                {
                   item.Kullanici = connection.Query<KullaniciViewModel>("select * from kullanicilar where id=@id",new {id= item.alankisi } ).First();
                }
                
                return View(model);
                //     connection.Execute("delete from sahiplik");

            }
        }
        
    }
}
