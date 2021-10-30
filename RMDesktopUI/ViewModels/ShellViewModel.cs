using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using RMDesktopUI.EventModels;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Models;

namespace RMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggedInUserModel _userModel;
        private IApiHelper _apiHelper;

        public ShellViewModel(IEventAggregator eventAggregator, ILoggedInUserModel userModel,
            IApiHelper apiHelper)
        {
            _eventAggregator = eventAggregator;
            _userModel = userModel;
            _apiHelper = apiHelper;

            _eventAggregator.SubscribeOnPublishedThread(this);


            ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());

        }

        public void ExitApplication()
        {
            TryCloseAsync();
        }

        public async Task UserManagement()
        {
            await ActivateItemAsync(IoC.Get<UserDisplayViewModel>(), new CancellationToken());
        }

        public async Task LogOut()
        {
            _userModel.ResetUserModel();
            _apiHelper.LogOffUser();

            await ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public async Task SalesAction()
        {
            await ActivateItemAsync(IoC.Get<SalesViewModel>(), new CancellationToken());
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
            await ActivateItemAsync(IoC.Get<SalesViewModel>(), cancellationToken);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
