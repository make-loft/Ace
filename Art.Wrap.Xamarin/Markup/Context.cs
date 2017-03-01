using System;
using Aero.Evocators;
using Aero.Input;
using Aero.Markup.Patterns;
using Xamarin.Forms;

namespace Aero.Markup
{
    public class Context : AMarkupExtension
    {
        private readonly Mediator _mediator = new Mediator();

        public Type StoreKey { get; set; }

        public string Key { get; set; }

        public override object Convert()
        {
            Target.TargetObject.Of<BindableObject>().BindingContextChanged += (sender, args) =>
            {
                var evocator = GetCommandEvocator(Target.TargetObject.Of<BindableObject>().BindingContext);
                _mediator.Initialize(Target.TargetObject, evocator);
            };

            return _mediator;
        }

        private CommandEvocator GetCommandEvocator(object dataContext)
        {
            var context = StoreKey == null
                ? dataContext as ContextObject
                : new Store {Key = StoreKey}.ProvideValue(null) as ContextObject;
            if (context == null) return null;
            var command = Aero.Context.Get(Key);
            return command == null ? null : context[command];
        }
    }
}