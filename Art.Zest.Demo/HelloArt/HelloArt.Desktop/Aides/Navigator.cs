using HelloArt.Views;

namespace HelloArt.Aides
{
    public static class Navigator
    {
        public static void Navigate(object path)
        {
            new TestView().Show();
        }
    }
}
