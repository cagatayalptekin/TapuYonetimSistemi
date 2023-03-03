namespace Tapudb.Models
{
    public class KayitViewModel
    {
        public int Id { get; set; }
        public int sayi { get; set; }
        public int alankisi { get; set; }
        public int verenkisi { get; set; }
        public int tapuid { get; set; }
        public float fiyat  { get; set; }
        public KullaniciViewModel Kullanici { get; set; }
        public KullaniciViewModel Kullanici2 { get; set; }
        public TapuViewModel tapu { get; set; }
        public int para { get; set; }
    }
}
