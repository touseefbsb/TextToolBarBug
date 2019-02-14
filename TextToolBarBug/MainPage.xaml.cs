﻿using TextToolBarBug.Services;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TextToolBarBug
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage() => InitializeComponent();

        private async void OpenNotes_Cliked(object sender, RoutedEventArgs e)
        {
            await WindowManagerService.Current
          .TryShowAsViewModeAsync("", typeof(NotePage),  ApplicationViewMode.CompactOverlay);

        }
    }
}
