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

			this[() => Kisses].Changed += args =>
			{
				Context.Get("KissGirl").EvokeCanExecuteChanged();
				Context.Get("KissGuy").EvokeCanExecuteChanged();
			};

			this[Context.Get("KissGirl")].CanExecute += args => 
				args.CanExecute = Kisses > girlViewModel.Kisses - 2;

			this[Context.Get("KissGirl")].Executed += args => 
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

			this[() => Kisses].Changed += args =>
			{
				Context.Get("KissGirl").EvokeCanExecuteChanged();
				Context.Get("KissGuy").EvokeCanExecuteChanged();
			};

			this[Context.Get("KissGuy")].CanExecute += args =>
				args.CanExecute = Kisses > guyViewModel.Kisses - 3;

			this[Context.Get("KissGuy")].Executed += args =>
				guyViewModel.Kisses++;
		}
	}
}
