global using System;
global using System.Linq;
global using System.Collections.Generic;
global using System.Threading.Tasks;

global using Command = Ace.Input.Command;

#if MAUI
global using Microsoft.Maui;
global using Microsoft.Maui.Controls;
global using Microsoft.Maui.Controls.Xaml;
global using Microsoft.Maui.Graphics;

//global using Element = Microsoft.Maui.Controls.Element;
//global using DependencyObject = Microsoft.Maui.Controls.BindableObject;
global using DependencyProperty = Microsoft.Maui.Controls.BindableProperty;
global using Property = Microsoft.Maui.Controls.BindableProperty;
global using Map = Ace.Replication.Models.Map;

global using ContextElement = Microsoft.Maui.Controls.View;

//using PropertyChangingEventHandler = System.ComponentModel.PropertyChangingEventHandler;
#endif

#if XAMARIN
global using Xamarin.Forms;
global using Xamarin.Forms.Xaml;

//global using FrameworkElement = Xamarin.Forms.Element;
//global using DependencyObject = Xamarin.Forms.Element;
global using Property = Xamarin.Forms.BindableProperty;
global using DependencyProperty = Xamarin.Forms.BindableProperty;

global using ContextElement = Xamarin.Forms.View;

global using PropertyChangingEventHandler = System.ComponentModel.PropertyChangingEventHandler;

using IValueConverter = Xamarin.Forms.IValueConverter;
#endif

#if DESKTOP
global using System.Windows.Controls;
global using System.Windows.Data;

global using DependencyObject = System.Windows.DependencyObject;
global using BindableObject = System.Windows.DependencyObject;

global using DependencyProperty = System.Windows.DependencyProperty;
global using Property = System.Windows.DependencyProperty;

global using View = System.Windows.FrameworkElement;
#endif