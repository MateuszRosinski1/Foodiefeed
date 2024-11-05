
using System.IO;

namespace Foodiefeed.views.windows.contentview;

public partial class PostImageView : ContentView
{
	public static BindableProperty ImageSourceProperty =
		BindableProperty.Create(nameof(ImageSource),typeof(string),typeof(PostImageView),default(string),propertyChanged: OnImageSourceChanged);

    private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostImageView)bindable;
		
		if(newValue is string filePath && !string.IsNullOrWhiteSpace(filePath))
		{
			byte[] data;

			using(var filestream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				using (var memorystream = new MemoryStream())
				{
					filestream.CopyTo(memorystream);
                    memorystream.Position = 0;
                    data = memorystream.ToArray();
				}
			}

			view.image.Source = Microsoft.Maui.Controls.ImageSource.FromStream(() => new MemoryStream(data));
		}
    }

    public string ImageSource
	{
		get => (string)GetValue(ImageSourceProperty);
		set => SetValue(ImageSourceProperty, value);
	}

	public PostImageView()
	{
		InitializeComponent();
	}
}