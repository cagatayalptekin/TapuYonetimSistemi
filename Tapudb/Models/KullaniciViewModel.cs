namespace Tapudb.Models
{
    public class KullaniciViewModel
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        
        public double para { get; set; }
        public IEnumerable<TapuViewModel> tapular { get; set; }
       
    }
}
