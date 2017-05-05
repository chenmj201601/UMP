namespace UMPS3604.Models
{
    public class PanelItem
    {
        public int PanelId { get; set; }
        public string Name { get; set; }
        public string ContentId { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }

        public bool IsVisible { get; set; }
        public bool CanClose { get; set; }
    }
}
