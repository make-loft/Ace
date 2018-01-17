using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using System.Windows.Markup;

[assembly: AssemblyTitle("Ace.Zest")]
[assembly: AssemblyDescription("http://makeloft.xyz")]
//[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Makeloft Studio")]
[assembly: AssemblyProduct("Ace Framework")]
[assembly: AssemblyCopyright("Copyright © Makeloft Studio")]
[assembly: AssemblyTrademark("Ace Framework")]
//[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]


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

#if !XAMARIN
// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("6daeb5df-e3e1-4745-b274-bbf44d6906d5")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Ace.Dictionaries")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Ace.Converters")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Ace.Markup")]
[assembly: XmlnsPrefix("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "m")]
#endif