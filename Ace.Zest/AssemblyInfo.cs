#if XAMARIN
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Skip)]
[assembly: XmlnsDefinition("ace", "Ace.Presentation")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Ace.Presentation")]
#else
using System.Reflection;
using System.Windows.Markup;
#if MAUI
using XmlnsPrefixAttribute = Microsoft.Maui.Controls.XmlnsPrefixAttribute;
#else

// Version information for an assembly consists of the following four values:
//
//	  Major Version
//	  Minor Version 
//	  Build Number
//	  Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("3.3.0.0")]
[assembly: AssemblyFileVersion("3.3.0.0")]
//[assembly: AssemblyInformationalVersion("3.3.0.0")]
[assembly: AssemblyCompany("Makeloft Studio")]
[assembly: AssemblyProduct("Ace Framework")]
//[assembly: AssemblyCopyright("© Makeloft Studio")]
//[assembly: AssemblyTrademark("Ace Framework")]
//[assembly: AssemblyTitle("Ace.Zest")]
//[assembly: AssemblyDescription("Developed by Makeloft Studio")]
//[assembly: AssemblyConfiguration("")]
//[assembly: AssemblyCulture("")]
#endif

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
// The following GUID is for the ID of the typelib if this project is exposed to COM
//[assembly: System.Runtime.InteropServices.Guid("6daeb5df-e3e1-4745-b274-bbf44d6906d5")]
//[assembly: System.Runtime.InteropServices.ComVisible(false)]

[assembly: XmlnsDefinition("http://xamarin.com/schemas/2014/forms", "Xamarin.Forms")]
#endif

//[assembly: XmlnsDefinition("ace", "Ace.Controls")]
//[assembly: XmlnsDefinition("ace", "Ace.Dictionaries")]
//[assembly: XmlnsDefinition("ace", "Ace.Markup.Converters")]
//[assembly: XmlnsDefinition("ace", "Ace.Markup")]
//[assembly: XmlnsPrefix("ace", "ace")]

//[assembly: XmlnsDefinition("http://xamarin.com/schemas/2014/forms", "Ace.Controls")]
//[assembly: XmlnsDefinition("http://xamarin.com/schemas/2014/forms", "Ace.Dictionaries")]
//[assembly: XmlnsDefinition("http://xamarin.com/schemas/2014/forms", "Ace.Markup.Converters")]
//[assembly: XmlnsDefinition("http://xamarin.com/schemas/2014/forms", "Ace.Markup")]
//[assembly: XmlnsPrefix("http://xamarin.com/schemas/2014/forms", "xamarin")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui", "Ace.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui", "Ace.Dictionaries")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui", "Ace.Markup.Converters")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui", "Ace.Markup")]
[assembly: XmlnsPrefix("http://schemas.microsoft.com/dotnet/2021/maui", "maui")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "Ace.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "Ace.Dictionaries")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "Ace.Markup.Converters")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "Ace.Markup")]
[assembly: XmlnsPrefix("http://schemas.microsoft.com/dotnet/maui/global", "maui")]

//[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Ace.Controls")]
//[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Ace.Dictionaries")]
//[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Ace.Markup.Converters")]
//[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Ace.Markup")]
//[assembly: XmlnsPrefix("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "m")]