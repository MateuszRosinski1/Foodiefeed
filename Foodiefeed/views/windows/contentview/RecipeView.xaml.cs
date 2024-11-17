using System.Windows.Input;

namespace Foodiefeed.views.windows.contentview;

public partial class RecipeView : ContentView
{

	public static readonly BindableProperty UsernameProperty =
        BindableProperty.Create(nameof(Username), typeof(string), typeof(RecipeView), default(string), propertyChanged: OnUsernameChanged);

    public static readonly BindableProperty ImageProperty =
        BindableProperty.Create(nameof(Image), typeof(string), typeof(RecipeView), default(string),propertyChanged: OnImageChanged);

    public static readonly BindableProperty RecipeContentProperty =
        BindableProperty.Create(nameof(RecipeContent), typeof(string), typeof(RecipeView), default(string),propertyChanged: OnContentChanged);

    public static readonly BindableProperty ProductsProperty =
        BindableProperty.Create(nameof(Products), typeof(List<string>), typeof(RecipeView), default(List<string>));

    public static readonly BindableProperty IdProperty =
        BindableProperty.Create(nameof(Id), typeof(string), typeof(RecipeView), default(string));

    public static readonly BindableProperty ContentVisibleProperty =
        BindableProperty.Create(nameof(ContentVisible), typeof(bool), typeof(RecipeView), default(bool));

    public static readonly BindableProperty DeleteCommandProperty =
        BindableProperty.Create(nameof(DeleteCommand),typeof(ICommand),typeof(RecipeView),default(ICommand));

    public string Username 
    { 
        get => (string)GetValue(UsernameProperty);
        set => SetValue(UsernameProperty, value);
    }

    public string Image 
    {
        get => (string)GetValue(ImageProperty); 
        set => SetValue(ImageProperty, value);
    }

    public string RecipeContent 
    { 
        get => (string)GetValue(RecipeContentProperty);
        set => SetValue(RecipeContentProperty, value);
    }

    public List<string> Products 
    {
        get => (List<string>)GetValue(ProductsProperty); 
        set => SetValue(ProductsProperty, value);
    }

    public string Id 
    {  
        get => (string)GetValue(IdProperty); 
        set => SetValue(IdProperty, value);
    }

    public bool ContentVisible { 
        get => (bool)GetValue(ContentVisibleProperty);
        set => SetValue(ContentVisibleProperty, value);
    }

    public ICommand DeleteCommand
    {
        get => (ICommand)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public RecipeView()
	{
		InitializeComponent();
        this.ContentVisible = true;
	}

    private static void OnUsernameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (RecipeView)bindable;
        view.usernameLabel.Text = newValue as string;
    }

    private static void OnImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (RecipeView)bindable;
        if (newValue is null) return;

        var newValueString = newValue as string;

        var imageBytes = Convert.FromBase64String(newValueString);

        view.image.Source = ImageSource.FromStream(() =>
        {
            var stream = new MemoryStream(imageBytes);
            stream.Position = 0;
            return stream;
        });
    }

    private static void OnContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (RecipeView)bindable;
        view.contentLabel.Text = newValue as string;
    }

    private void SwitchContent(object sender, EventArgs e)
    {
        this.ContentVisible = !this.ContentVisible;
        if(this.ContentVisible)
        {
            ChangeContentButton.Text = "Products";
            contentLabel.Text = RecipeContent;
        }
        else
        {
            ChangeContentButton.Text = "Recipe";
            string productString = string.Empty;
            foreach(var product in Products)
            {
                productString += product + ",";               
            }

            if (!string.IsNullOrEmpty(productString))
            {
                productString.TrimEnd(',');
            }
            contentLabel.Text = productString;
        }
    }

    private void DeleteButtonClicked(object sender, EventArgs e)
    {
        if (DeleteCommand?.CanExecute(Id) ?? false)
        {
            DeleteCommand.Execute(Id);
        }
    }
}