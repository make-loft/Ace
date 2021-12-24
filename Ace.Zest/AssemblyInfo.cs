using System.Reflection;
using System.Windows.Markup;

// Version information for an assembly consists of the following four values:
//
//	  Major Version
//	  Minor Version 
//	  Build Number
//	  Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("2.0.*")]
[assembly: AssemblyFileVersion("2.0.0.0")]
[assembly: AssemblyCompany("Makeloft Studio")]
[assembly: AssemblyProduct("Ace Framework")]
[assembly: AssemblyCopyright("Copyright © Makeloft Studio")]
[assembly: AssemblyTrademark("Ace Framework")]
[assembly: AssemblyTitle("Ace.Zest")]
[assembly: AssemblyDescription("http://makeloft.xyz")]
//[assembly: AssemblyConfiguration("")]
//[assembly: AssemblyCulture("")]

#if !XAMARIN
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Ace.Dictionaries")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Ace.Converters")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Ace.Markup")]
[assembly: XmlnsPrefix("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "m")]
#endif

[assembly: XmlnsPrefix("http://xamarin.com/schemas/2014/forms", "xamarin")]
[assembly: XmlnsDefinition("http://xamarin.com/schemas/2014/forms", "Ace.Markup")]
[assembly: XmlnsDefinition("http://xamarin.com/schemas/2014/forms", "Ace.Converters")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
// The following GUID is for the ID of the typelib if this project is exposed to COM
//[assembly: System.Runtime.InteropServices.Guid("6daeb5df-e3e1-4745-b274-bbf44d6906d5")]
//[assembly: System.Runtime.InteropServices.ComVisible(false)]