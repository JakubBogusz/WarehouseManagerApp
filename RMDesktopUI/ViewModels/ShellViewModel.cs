using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using RMDesktopUI.EventModels;
using RMDesktopUI.Library.Models;

namespace RMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        // TODO - is something breaks had to be without readonly before
        private readonly IEventAggregator _eventAggregator;
        private readonly SalesViewModel _salesViewModel;
        private readonly ILoggedInUserModel _userModel;

        public ShellViewModel(IEventAggregator eventAggregator, SalesViewModel salesViewModel, ILoggedInUserModel userModel)
        {
            _eventAggregator = eventAggregator;
            _salesViewModel = salesViewModel;
            _userModel = userModel;

            _eventAggregator.Subscribe(this);
            
            
            ActivateItemAsync(IoC.Get<LoginViewModel>()); 
          
        }

        public void ExitApplication()
        {
            TryCloseAsync();
        }

        public void LogOut()
        {
            _userModel.LogOffUser();
            ActivateItemAsync(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public bool IsLoggedIn
        {
            get
            {
                bool output = false;

                if (string.IsNullOrWhiteSpace(_userModel.Token) == false)
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
             
             await ActivateItemAsync(_salesViewModel, cancellationToken);
             NotifyOfPropertyChange(() => IsLoggedIn);
             //NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
