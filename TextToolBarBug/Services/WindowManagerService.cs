using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TextToolBarBug.Services
{
    public delegate void ViewClosedHandler(ViewLifetimeControl viewControl, EventArgs e);

    // For instructions on using this service see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/features/multiple-views.md
    // More details about showing multiple views at https://docs.microsoft.com/windows/uwp/design/layout/show-multiple-views
    public class WindowManagerService
    {
        private static WindowManagerService _current;

        public static WindowManagerService Current => _current ?? (_current = new WindowManagerService());

        // Contains all the opened secondary views.
        public ObservableCollection<ViewLifetimeControl> SecondaryViews { get; } = new ObservableCollection<ViewLifetimeControl>();

        public int MainViewId => ApplicationView.GetForCurrentView().Id;

        public CoreDispatcher MainDispatcher => Window.Current.Dispatcher;
        public readonly static Windows.Foundation.Size MediumSize = new Windows.Foundation.Size(520, 400);

        public async Task<ViewLifetimeControl> TryShowAsViewModeAsync(string windowTitle, Type pageType, ApplicationViewMode viewMode = ApplicationViewMode.Default)
        {
            ViewLifetimeControl viewControl = await CreateViewLifetimeControlAsync(windowTitle, pageType);
            SecondaryViews.Add(viewControl);
            viewControl.StartViewInUse();

            ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(viewMode);
            compactOptions.CustomSize = MediumSize;

            var viewShown = await ApplicationViewSwitcher.TryShowAsViewModeAsync(viewControl.Id, viewMode, compactOptions);
            viewControl.StopViewInUse();
            return viewControl;
        } 

        private async Task<ViewLifetimeControl> CreateViewLifetimeControlAsync(string windowTitle, Type pageType)
        {
            ViewLifetimeControl viewControl = null;
            await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                viewControl = ViewLifetimeControl.CreateForCurrentView();
                viewControl.Title = windowTitle;
                viewControl.StartViewInUse();
                var frame = new Frame();
                frame.RequestedTheme = Application.Current.RequestedTheme == ApplicationTheme.Dark ? 
                                       ElementTheme.Dark : ElementTheme.Light;
                frame.Navigate(pageType, viewControl);
                Window.Current.Content = frame;
                Window.Current.Activate();
                ApplicationView.GetForCurrentView().Title = viewControl.Title;
            });
            return viewControl;
        }

        public bool IsWindowOpen(string windowTitle) => SecondaryViews.Any(v => v.Title == windowTitle);
    }
}
