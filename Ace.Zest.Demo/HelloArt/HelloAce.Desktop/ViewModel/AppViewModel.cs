using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Ace;
using HelloAce.Aides;

namespace HelloAce.ViewModel
{
	[DataContract]
	public class AppViewModel : ContextObject, IExposable
	{
		[DataMember]
		public double Number
		{
			get { return Get(() => Number); }
			set { Set(() => Number, value); }
		}

		[DataMember]
		public string Mouse
		{
			get { return Get(() => Mouse, "Mouse"); }
			set { Set(() => Mouse, value); }
		}

		[DataMember]
		public string Rabbit
		{
			get { return Get(() => Rabbit, "Rabbit"); }
			set { Set(() => Rabbit, value); }
		}

		public void Expose()
		{
			// sync simple validation way with IDataErrorInfo
			// Text="{Binding Rabbit, Mode=TwoWay, ValidatesOnDataErrors=True, NotifyOnValidationError=True}"
			this[() => Rabbit].ValidationRules += s =>
				Error = 5 < Rabbit.Length && Rabbit.Length < 20 ? null : "Invalid Length";

			// async validation way with INotifyDataErrorInfo
			// Text="{Binding Mouse, Mode=TwoWay, ValidatesOnNotifyDataErrors=True, NotifyOnValidationError=True}"
			this[() => Mouse].PropertyChanged += (sender, args) => RaiseErrorsChanged(() => Mouse);
			this[() => Mouse].ErrorsChanged += (sender, args) => HasErrors = !(5 < Mouse.Length && Mouse.Length < 20);
			this[() => Mouse].ValidationRules += s => 5 < Mouse.Length && Mouse.Length < 20 ? null : "Invalid Length";
			RaiseErrorsChanged(() => Mouse);

			//PropertyChanged += (sender, args) => Context.Make.RaiseCanExecuteChanged();
			this[() => Rabbit].PropertyChanged += (sender, args) => Context.Make.RaiseCanExecuteChanged();
			this[() => Mouse].PropertyChanged += (sender, args) => Context.Make.RaiseCanExecuteChanged();

			this[Context.Make].CanExecute += (sender, args) => args.CanExecute = !HasErrors;
			this[Context.Get("Make")].Executed += (sender, args) =>
				MessageBox.Show("Make!");
			this[Context.Make].Executed += async (sender, args) =>
				await Task.Factory.StartNew(() => MessageBox.Show("Make async!"));

			this[NavigationCommands.GoToPage].CanExecute += (sender, args) => args.CanExecute = Error == null;
			this[NavigationCommands.GoToPage].Executed += (sender, args) => Navigator.Navigate(args.Parameter);

			this[Context.Get("Hello")].Executed += (sender, args) => MessageBox.Show("Hello Command!");
		}
	}
}