using System.ComponentModel.DataAnnotations;

namespace Foodiefeed.views.windows.contentview;

public partial class PostView : ContentView
{
	[MaxLength(10)]
    public byte[] Images { get; set; }

    public PostView()
	{
		InitializeComponent();	
	}
	
}