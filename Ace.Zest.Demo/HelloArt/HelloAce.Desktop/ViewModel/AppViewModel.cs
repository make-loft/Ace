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
			get => Get(() => Number);
			set => Set(() => Number, value);
		}

		[DataMember]
		public string Mouse
		{
			get => Get(() => Mouse, "Mouse");
			set => Set(() => Mouse, value);
		}

		[DataMember]
		public string Rabbit
		{
			get => Get(() => Rabbit, "Rabbit");
			set => Set(() => Rabbit, value);
		}

		public void Expose()
		{
			// sync simple validation way with IDataErrorInfo
			// Text="{Binding Rabbit, Mode=TwoWay, ValidatesOnDataErrors=True, NotifyOnValidationError=True}"
			this[() => Rabbit].ValidationRules += s =>
				Error = 5 < Rabbit.Length && Rabbit.Length < 20 ? null : "Invalid Length";

			// async validation way with INotifyDataErrorInfo
			// Text="{Binding Mouse, Mode=TwoWay, ValidatesOnNotifyDataErrors=True, NotifyOnValidationError=True}"
			this[() => Mouse].Changed += args => EvokeErrorsChanged(() => Mouse);
			this[() => Mouse].ErrorsChanged += args => HasErrors = !(5 < Mouse.Length && Mouse.Length < 20);
			this[() => Mouse].ValidationRules += s => 5 < Mouse.Length && Mouse.Length < 20 ? null : "Invalid Length";

			EvokeErrorsChanged(() => Mouse);

			//Changed += (sender, args) => Context.Make.EvokeCanExecuteChanged();
			this[() => Rabbit].Changed += args => Context.Make.EvokeCanExecuteChanged();
			this[() => Mouse].Changed += args => Context.Make.EvokeCanExecuteChanged();

			this[Context.Make].CanExecute += args => args.CanExecute = !HasErrors;
			this[Context.Make].Executed += async args => await Task.Factory.StartNew(() => MessageBox.Show("Make async!"));

			this[Context.Get("Hello")].Executed += args => MessageBox.Show("Hello Command!");

			this[NavigationCommands.GoToPage].CanExecute += args => args.CanExecute = Error == null;
			this[NavigationCommands.GoToPage].Executed += args => Navigator.Navigate(args.Parameter);
		}
	}
}