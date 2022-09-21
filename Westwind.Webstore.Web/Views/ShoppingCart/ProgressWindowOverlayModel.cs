namespace Westwind.Webstore.Web;

public class ProgressWindowOverlayModel
{
    public string Title { get; set;  }= "Processing Payment";
    public string SubTitle { get; set; } = "this may take a few seconds...";
    public string Icon { get; set; } = "fad fa-credit-card-front";
}
