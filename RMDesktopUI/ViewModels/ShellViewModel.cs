using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using RMDesktopUI.EventModels;

namespace RMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        // TODO - is something breaks had to be without readonly before
        private readonly IEventAggregator _eventAggregator;
        private readonly SalesViewModel _salesViewModel;

        public ShellViewModel(IEventAggregator eventAggregator, SalesViewModel salesViewModel)
        {
            _eventAggregator = eventAggregator;
            _salesViewModel = salesViewModel;

            _eventAggregator.Subscribe(this);
            
            
            ActivateItemAsync(IoC.Get<LoginViewModel>()); 
          
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
             
             await ActivateItemAsync(_salesViewModel, cancellationToken);
             //NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
