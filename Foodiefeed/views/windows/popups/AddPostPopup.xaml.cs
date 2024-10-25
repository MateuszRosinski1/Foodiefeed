using CommunityToolkit.Maui.Views;

namespace Foodiefeed.views.windows.popups;

public partial class AddPostPopup : Popup
{
	public AddPostPopup()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
		var fileresult = await FilePicker.Default.PickAsync(new PickOptions
		{
			PickerTitle = "wybierz plik",
			FileTypes = FilePickerFileType.Images
		});
    }
}