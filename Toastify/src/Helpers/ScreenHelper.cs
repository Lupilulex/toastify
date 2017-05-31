﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using Toastify.UI;

namespace Toastify.Helpers
{
    internal static class ScreenHelper
    {
        private const int SCREEN_RIGHT_MARGIN = 0;
        private const int SCREEN_TOP_MARGIN = 5;

        public static Point GetDPIRatios()
        {
            var presentationSource = PresentationSource.FromVisual(Toast.Current);

            // if we hit this then Settings were loaded before the Toast window was
            Debug.Assert(presentationSource != null);

            return new Point(presentationSource.CompositionTarget?.TransformToDevice.M11 ?? 1.0,
                             presentationSource.CompositionTarget?.TransformToDevice.M22 ?? 1.0);
        }

        public static Point GetDefaultToastPosition(double width, double height)
        {
            var screenRect = Screen.PrimaryScreen.WorkingArea;

            var dpiRatio = GetDPIRatios();

            return new Point(screenRect.Width / dpiRatio.X - width - SCREEN_RIGHT_MARGIN,
                             screenRect.Height / dpiRatio.Y - height - SCREEN_TOP_MARGIN);
        }
    }
}