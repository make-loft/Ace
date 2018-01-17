using System.Runtime.Serialization;
using Ace;

namespace HelloAce.ViewModel
{
	[DataContract]
	public class GuyViewModel : ContextObject, IExposable
	{
		[DataMember]
		public int Kisses
		{
			get { return Get(() => Kisses); }
			set { Set(() => Kisses, value); }
		}

		public void Expose()
		{
			var girlViewModel = Store.Get<GirlViewModel>();

			this[() => Kisses].PropertyChanged += (sender, args) =>
			{
				Context.Get("KissGirl").RaiseCanExecuteChanged();
				Context.Get("KissGuy").RaiseCanExecuteChanged();
			};

			this[Context.Get("KissGirl")].CanExecute += (sender, args) => 
				args.CanExecute = Kisses > girlViewModel.Kisses - 2;

			this[Context.Get("KissGirl")].Executed += (sender, args) => 
				girlViewModel.Kisses++;
		}
	}

	[DataContract]
	public class GirlViewModel : ContextObject, IExposable
	{
		[DataMember]
		public int Kisses
		{
			get { return Get(() => Kisses); }
			set { Set(() => Kisses, value); }
		}

		public void Expose()
		{
			var guyViewModel = Store.Get<GuyViewModel>();

			this[() => Kisses].PropertyChanged += (sender, args) =>
			{
				Context.Get("KissGirl").RaiseCanExecuteChanged();
				Context.Get("KissGuy").RaiseCanExecuteChanged();
			};

			this[Context.Get("KissGuy")].CanExecute += (sender, args) =>
				args.CanExecute = Kisses > guyViewModel.Kisses - 3;

			this[Context.Get("KissGuy")].Executed += (sender, args) =>
				guyViewModel.Kisses++;
		}
	}
}
