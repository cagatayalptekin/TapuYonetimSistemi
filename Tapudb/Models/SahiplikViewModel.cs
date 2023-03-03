namespace Tapudb.Models
{
    public class SahiplikViewModel
    {
        public TapuViewModel tapu { get; set; }
        public KullaniciViewModel kullanici { get; set; }
        public int kullaniciid { get; set; }
        public int tapuid { get; set; }
        public int sayi { get; set; }

        public DateTime AlisTarihi { get; set; }
    }
}
