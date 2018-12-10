namespace Ace.Adapters
{
    public class ResourceDictionary : Xamarin.Forms.ResourceDictionary {}

    public class StackPanel : Xamarin.Forms.StackLayout
    {
        public object DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
    }
    
    public class PresentationAdapter
    {
        
    }
}