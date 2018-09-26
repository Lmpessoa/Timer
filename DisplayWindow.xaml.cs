/*
 * Copyright (c) 2018 Leonardo Pessoa
 * https://lmpessoa.com
 * 
 * Licensed under the Leeow Open Source License, Version 1.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of the 
 * License at:
 * 
 * https://www.leeow.io/licences/1.0
 * 
 * UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING, SOFTWARE DISTRIBUTED UNDER
 * THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, EITHER EXPRESS OR IMPLIED. See the License for the specific language
 * governing permissions and limitations under the License.
 */
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Timer {

    public partial class DisplayWindow : Window {

        public DisplayWindow() {
            InitializeComponent();
        }

        public double ImageWidth => imageBox.Width;

        public bool CanClose { get; set; } = false;

        public void SetImage(ImageSource drawing) {
            imageBox.Source = drawing;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.F11) {
                if (WindowState == WindowState.Normal) {
                    WindowState = WindowState.Maximized;
                    WindowStyle = WindowStyle.None;
                    Topmost = true;
                } else {
                    WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    Topmost = false;
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            e.Cancel = !CanClose;
        }
    }
}
