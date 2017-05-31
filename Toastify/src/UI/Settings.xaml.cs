﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Toastify.Core;
using Toastify.Services;

namespace Toastify.UI
{
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class Settings : Window
    {
        public SettingsXml settings;
        private readonly Toast toast;

        private List<Key> modifierKeys = new List<Key> { Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.Right, Key.LeftShift, Key.RightShift, Key.LWin, Key.RWin, Key.System };

        private static Settings _current;

        public static void Launch(Toast toast)
        {
            if (_current != null)
                _current.Activate();
            else
                new Settings(toast).ShowDialog();
        }

        private Settings(Toast toast)
        {
            Telemetry.TrackEvent(TelemetryCategory.General, Telemetry.TelemetryEvent.SettingsLaunched);

            this.settings = SettingsXml.Instance.Clone();
            this.toast = toast;

            this.InitializeComponent();

            //Data context initialisation
            this.GeneralGrid.DataContext = this.settings;

            //Slider initialisation
            try
            {
                this.slTopColor.Value = byte.Parse(this.settings.ToastColorTop.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                this.slBottomColor.Value = byte.Parse(this.settings.ToastColorBottom.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                this.slBorderColor.Value = byte.Parse(this.settings.ToastBorderColor.Substring(1, 2), NumberStyles.AllowHexSpecifier);
            }
            catch
            {
                // ignored
            }

            if (_current == null)
                _current = this;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (ReferenceEquals(_current, this))
                _current = null;
        }

        //Change Color button click events
        private void ChangeColorTop_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            string alpha = this.settings.ToastColorTop.Substring(1, 2);
            MyDialog.Color = HexToColor(this.settings.ToastColorTop);
            MyDialog.ShowDialog();
            this.settings.ToastColorTop = "#" + alpha + MyDialog.Color.R.ToString("X2") + MyDialog.Color.G.ToString("X2") + MyDialog.Color.B.ToString("X2");
        }

        private void ChangeColorBottom_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            string alpha = this.settings.ToastColorBottom.Substring(1, 2);
            MyDialog.Color = HexToColor(this.settings.ToastColorBottom);
            MyDialog.ShowDialog();
            this.settings.ToastColorBottom = "#" + alpha + MyDialog.Color.R.ToString("X2") + MyDialog.Color.G.ToString("X2") + MyDialog.Color.B.ToString("X2");
        }

        private void ChangeBorderColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            string alpha = this.settings.ToastBorderColor.Substring(1, 2);
            MyDialog.Color = HexToColor(this.settings.ToastBorderColor);
            MyDialog.ShowDialog();
            this.settings.ToastBorderColor = "#" + alpha + MyDialog.Color.R.ToString("X2") + MyDialog.Color.G.ToString("X2") + MyDialog.Color.B.ToString("X2");
        }

        //Default and Save blick events
        private void Default_Click(object sender, RoutedEventArgs e)
        {
            this.settings.Default();
            this.SaveAndApplySettings();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            this.SaveAndApplySettings();
        }

        private void SaveAndApplySettings()
        {
            this.settings.Save(true);

            this.toast.InitToast();
            this.toast.DisplayAction(SpotifyAction.SettingsSaved, null);
        }

        //Text box Mouse Wheel events
        private void CornerTopLeft_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                this.settings.ToastBorderCornerRadiusTopLeft += 0.1;
            }
            else if (this.settings.ToastBorderCornerRadiusTopLeft >= 0.1)
                this.settings.ToastBorderCornerRadiusTopLeft -= 0.1;
        }

        private void CornerTopRight_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.ToastBorderCornerRadiusTopRight += 0.1;
            else if (this.settings.ToastBorderCornerRadiusTopLeft >= 0.1)
                this.settings.ToastBorderCornerRadiusTopRight -= 0.1;
        }

        private void CornerBottomRight_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.ToastBorderCornerRadiusBottomRight += 0.1;
            else if (this.settings.ToastBorderCornerRadiusBottomRight >= 0.1)
                this.settings.ToastBorderCornerRadiusBottomRight -= 0.1;
        }

        private void CornerBottomLeft_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.ToastBorderCornerRadiusBottomLeft += 0.1;
            else if (this.settings.ToastBorderCornerRadiusBottomLeft >= 0.1)
                this.settings.ToastBorderCornerRadiusBottomLeft -= 0.1;
        }

        private void FadeOutTime_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.FadeOutTime += 10;
            else if (this.settings.FadeOutTime >= 10)
                this.settings.FadeOutTime -= 10;
        }

        private void BorderThickness_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.ToastBorderThickness++;
            else if (this.settings.ToastBorderThickness >= 1)
                this.settings.ToastBorderThickness--;
        }

        private void ToastWidth_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.ToastWidth += 5;
            else if (this.settings.ToastWidth >= 205)
                this.settings.ToastWidth -= 5;
        }

        private void ToastHeight_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.ToastHeight += 5;
            else if (this.settings.ToastHeight >= 70)
                this.settings.ToastHeight -= 5;
        }

        private void PositionLeft_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.PositionLeft++;
            else if (this.settings.PositionLeft > 0)
                this.settings.PositionLeft--;
        }

        private void PositionTop_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.PositionTop++;
            else if (this.settings.PositionTop > 0)
                this.settings.PositionTop--;
        }

        //Slider value changed events
        private void TopColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string transparency = Convert.ToByte(this.slTopColor.Value).ToString("X2");
            this.settings.ToastColorTop = "#" + transparency + this.settings.ToastColorTop.Substring(3);
        }

        private void BottomColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string transparency = Convert.ToByte(this.slBottomColor.Value).ToString("X2");
            this.settings.ToastColorBottom = "#" + transparency + this.settings.ToastColorBottom.Substring(3);
        }

        private void BorderColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string transparency = Convert.ToByte(this.slBorderColor.Value).ToString("X2");
            this.settings.ToastBorderColor = "#" + transparency + this.settings.ToastBorderColor.Substring(3);
        }

        // Hexadecimal to Color converter
        public static System.Drawing.Color HexToColor(string hexColor)
        {
            //Remove # if present
            if (hexColor.IndexOf('#') != -1)
                hexColor = hexColor.Replace("#", "");

            byte alpha = 0;
            byte red = 0;
            byte green = 0;
            byte blue = 0;

            if (hexColor.Length == 8)
            {
                //#RRGGBB
                alpha = byte.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                red = byte.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                green = byte.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
                blue = byte.Parse(hexColor.Substring(6, 2), NumberStyles.AllowHexSpecifier);
            }

            return System.Drawing.Color.FromArgb(alpha, red, green, blue);
        }

        private void TxtSingleKey_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;

            var key = e.Key;
            if (key == Key.System)
                key = e.SystemKey;

            this.txtSingleKey.Text = key.ToString();

            if (this.lstHotKeys.SelectedItem is Hotkey hotkey)
                hotkey.Key = key;
        }

        private void LstHotKeys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lstHotKeys.SelectedItem is Hotkey hotkey)
                this.txtSingleKey.Text = hotkey.Key.ToString();
        }

        private void BtnSaveTrackToFilePath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (SettingsXml.Instance.SaveTrackToFilePath != null)
            {
                dialog.FileName = SettingsXml.Instance.SaveTrackToFilePath;
            }

            dialog.CheckPathExists = true;
            dialog.CheckFileExists = false;
            dialog.ShowReadOnly = false;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.settings.SaveTrackToFilePath = dialog.FileName;
        }
    }
}