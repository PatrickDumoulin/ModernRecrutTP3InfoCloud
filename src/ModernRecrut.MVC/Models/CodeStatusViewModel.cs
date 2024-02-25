namespace ModernRecrut.MVC.Models
{
    public class CodeStatusViewModel
    {
        public string MessageErreur {  get; set; }
        public int CodeStatus { get; set; }
        public string IdRequete { get; set; }
        public bool MontrerIdRequete => !string.IsNullOrEmpty(IdRequete);
    }
}
