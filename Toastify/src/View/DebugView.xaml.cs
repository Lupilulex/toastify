﻿using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using Toastify.Model;

namespace Toastify.View
{
    // ReSharper disable once RedundantExtendsListEntry
    public partial class DebugView : Window
    {
        internal static DebugView Current { get; private set; }

        private Settings CurrentSettings { get { return Settings.Current; } }
        private Settings PreviewSettings { get; set; }

        public DebugView()
        {
            this.InitializeComponent();

            this.DataContext = this;
            Current = this;

            SettingsView.SettingsLaunched += this.SettingsView_SettingsLaunched;
            SettingsView.SettingsClosed += this.SettingsView_SettingsClosed;
        }

        internal static void Launch()
        {
            if (Current != null)
                return;

            Thread th = new Thread(() =>
            {
                DebugView debugView = new DebugView();
                debugView.Show();
                System.Windows.Threading.Dispatcher.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.IsBackground = true;
            th.Start();
        }

        private void ButtonPrintSettings_OnClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("\n======= DebugView =======");
            Debug.WriteLine("SETTINGS (CURRENT | (PREVIEW) | DEFAULT):");

            var properties = typeof(Settings).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                             .Where(p => p.PropertyType.GetInterfaces().Contains(typeof(ISettingValue)));

            if (this.PreviewSettings != null)
            {
                foreach (var property in properties)
                {
                    dynamic current = property.GetValue(Settings.Current);
                    dynamic preview = property.GetValue(this.PreviewSettings);
                    dynamic @default = property.GetValue(Settings.Default);

                    Debug.WriteLine($"{property.Name,-36}:  {current?.ToString(),-25} | {preview?.ToString(),-25} | {@default?.ToString(),-25}");
                }
            }
            else
            {
                foreach (var property in properties)
                {
                    dynamic current = property.GetValue(Settings.Current);
                    dynamic @default = property.GetValue(Settings.Default);

                    Debug.WriteLine($"{property.Name,-36}:  {current?.ToString(),-25} | {@default?.ToString(),-25}");
                }
            }

            Debug.WriteLine("=========================\n");
        }

        private void ButtonPrintCurrentHotkeys_OnClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("\n======= DebugView =======");
            Debug.WriteLine("CURRENT SETTINGS HOTKEYS:");
            if (this.CurrentSettings?.HotKeys != null)
            {
                foreach (var h in this.CurrentSettings.HotKeys)
                    Debug.WriteLine(h.ToString());
            }

            if (this.PreviewSettings != null)
            {
                Debug.WriteLine("\nPREVIEW SETTINGS HOTKEYS:");
                if (this.PreviewSettings.HotKeys != null)
                {
                    foreach (var h in this.PreviewSettings.HotKeys)
                        Debug.WriteLine(h.ToString());
                }
            }
            Debug.WriteLine("=========================\n");
        }

        private void DebugView_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            SettingsView.SettingsLaunched -= this.SettingsView_SettingsLaunched;

            Current = null;
        }

        private void SettingsView_SettingsLaunched(object sender, Events.SettingsViewLaunchedEventArgs e)
        {
            this.PreviewSettings = e.Settings;
        }

        private void SettingsView_SettingsClosed(object sender, System.EventArgs e)
        {
            this.PreviewSettings = null;
        }

        private void LogShowToastAction_OnChecked(object sender, RoutedEventArgs e)
        {
            if (ToastView.Current != null)
            {
                ToastView.Current.LogShowToastAction = true;
                this.cbLogShowToastAction.IsChecked = true;
            }
        }

        private void LogShowToastAction_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (ToastView.Current != null)
            {
                ToastView.Current.LogShowToastAction = false;
                this.cbLogShowToastAction.IsChecked = false;
            }
        }
    }
}