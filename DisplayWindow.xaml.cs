/*
 * Copyright (c) 2018 Leonardo Pessoa
 * https://lmpessoa.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
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
