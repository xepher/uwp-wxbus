using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Navicator
{
    /// <summary>
    /// Interface for navigation in an application.
    /// </summary>
    public interface INavigator
    {
        /// <summary>
        /// Gets whether there is a previous page to go back to.
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Goes back to the previous page.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Navigates to the page for a given view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to navigate to.</typeparam>
        /// <param name="parameter">An optional navigation parameter.</param>
        void NavigateToViewModel<TViewModel>(object parameter = null);

        /// <summary>
        /// Removes the back entry.
        /// </summary>
        void RemoveBackEntry();
    }
}
