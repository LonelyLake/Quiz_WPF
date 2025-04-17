using Quiz.ViewModels.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Services
{
    public class NavigationService
    {
        private Action<ViewModelBase> _navigate;

        public void SetNavigator(Action<ViewModelBase> navigator)
        {
            _navigate = navigator;
        }

        public void NavigateTo(ViewModelBase viewModel)
        {
            _navigate?.Invoke(viewModel);
        }
    }
}
