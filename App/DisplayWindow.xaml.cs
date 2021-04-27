/*
 * Copyright (c) 2021 Leonardo Pessoa
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Lmpessoa.Timer {

   public partial class DisplayWindow : Window {

      public DisplayWindow() => InitializeComponent();

      public double ImageWidth => imageBox.Width;

      public bool CanClose { get; set; } = false;

      public void SetImage(ImageSource drawing) => imageBox.Source = drawing;

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

      private void Window_Closing(object sender, CancelEventArgs e) => e.Cancel = !CanClose;
   }
}