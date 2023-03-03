using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Tapudb.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tapudb.Controllers
{
    public class SahiplikController : Controller
    {
        public IActionResult Index()
        {

            using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
            {
                var tapular = connection.Query<TapuViewModel>("select * from tapular");
                var sahiplikler = connection.Query<SahiplikViewModel>("select * from sahiplik");
               
              
                int i = 0;
                foreach (var item in sahiplikler)
                {

                    var alankullanici = connection.Query<KullaniciViewModel>("select * from kullanicilar where id=@id", new { id = item.kullaniciid });
                    var tapu = connection.Query<TapuViewModel>("select * from tapular where id=@id", new { id = item.tapuid });
                    if (alankullanici.Any() && tapu.Any())
                    {
                        item.kullanici = alankullanici.FirstOrDefault();
                        item.tapu = tapu.FirstOrDefault();
                    }
                    i++;

                }

                return View(sahiplikler);
            }
        }
            public IActionResult SahipSayisi()
            {
                
                using (var connection = new NpgsqlConnection("Server = localhost; Database = Tapudb; Port = 5432; User Id = postgres; password = cagatayalp4;"))
                {

                    var sahipsayisi = connection.Query<TapuViewModel>("select sahipid, count(*) as sayi from tapular  group by sahipid having count(*)>0");
                    foreach (var item in sahipsayisi)
                    {
                        var kullanici = connection.Query<KullaniciViewModel>("select * from kullanicilar where id=@id", new { id = item.sahipId });
                        if (kullanici.Any())
                        {
                            item.Kullanici = kullanici.FirstOrDefault();
                        }
                    }
                    
                    return View(sahipsayisi);
                }
            }
            


        }
}

