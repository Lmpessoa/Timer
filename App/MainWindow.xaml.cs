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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using msg = Lmpessoa.Timer.Properties.Resources;

namespace Lmpessoa.Timer {

   public partial class MainWindow : Window {

      private static readonly ResourceDictionary res = Application.Current.Resources;
      private static readonly DrawingImage startImg = (DrawingImage) res["StartImage"];
      private static readonly DrawingImage stopImg = (DrawingImage) res["StopImage"];

      private readonly Brush errorBorder = new SolidColorBrush(Colors.Red);
      private readonly Brush defaultBorder;

      private DisplayWindow display;
      private TimeSpan? yellowTime = null;
      private TimeSpan? redTime = null;
      private TimeSpan? startTime = null;
      private TimeSpan? actualTime = null;
      private bool mayChange = false;
      private bool isNegative = false;
      private bool finished = false;
      private DispatcherTimer timer;

      public MainWindow() {
         InitializeComponent();
         defaultBorder = startTimeBox.BorderBrush;
         display = new DisplayWindow();
         InitializeTimer();
         SetTimer_Click(this, null);
      }

      private void InitializeTimer() {
         timer = new DispatcherTimer() {
            Interval = new TimeSpan(0, 0, 0, 0, 500),
         };
         timer.Tick += StepTimer;
      }

      private void Window_Activated(object sender, EventArgs e) {
         if (!display.IsVisible) {
            display.Left = Left + Width;
            display.Top = Top;
            display.Show();
         }
      }

      private void Window_Closing(object sender, CancelEventArgs e) {
         display.CanClose = true;
         display.Close();
         display = null;
      }

      private void StartTimeBox_LostFocus(object sender, RoutedEventArgs e) {
         TextBoxLostFocus(startTimeBox, ref startTime);
         setTimer.IsEnabled = startTimeBox.BorderBrush != errorBorder;
      }

      private void StartTimeBox_KeyUp(object sender, KeyEventArgs e) {
         if (e.Key == Key.Enter) {
            StartTimeBox_LostFocus(sender, null);
            SetTimer_Click(sender, null);
            e.Handled = true;
         }
      }

      private void UseYellowTime_Checked(object sender, RoutedEventArgs e) => yellowTimeBox.IsEnabled = useYellowTime.IsChecked ?? false;

      private void YellowTimeBox_LostFocus(object sender, RoutedEventArgs e) => TextBoxLostFocus(yellowTimeBox, ref yellowTime);

      private void YellowTimeBox_KeyUp(object sender, KeyEventArgs e) {
         if (e.Key == Key.Enter) {
            YellowTimeBox_LostFocus(sender, null);
            e.Handled = true;
         }
      }

      private void UseRedTime_Checked(object sender, RoutedEventArgs e) => redTimeBox.IsEnabled = useRedTime.IsChecked ?? false;

      private void RedTimeBox_LostFocus(object sender, RoutedEventArgs e) => TextBoxLostFocus(redTimeBox, ref redTime);

      private void RedTimeBox_KeyUp(object sender, KeyEventArgs e) {
         if (e.Key == Key.Enter) {
            RedTimeBox_LostFocus(sender, null);
            e.Handled = true;
         }
      }

      private void TextBoxLostFocus(TextBox textBox, ref TimeSpan? timer) {
         string value = textBox.Text;
         try {
            if (value != null && value != "") {
               timer = ParseTimeSpan(value);
               textBox.Text = FormatTimeSpan(timer);
               textBox.BorderBrush = defaultBorder;
               textBox.ToolTip = null;
            } else {
               timer = null;
            }
         } catch (Exception ex) {
            textBox.BorderBrush = errorBorder;
            textBox.ToolTip = ex.Message;
            timer = null;
         }
      }

      private void SetTimer_Click(object sender, RoutedEventArgs e) {
         if (startStopText.Text == Timer.Properties.Resources.stop || actualTime == TimeSpan.Zero) {
            SetStartStopToStart();
            timer.Stop();
         }
         actualTime = startTime;
         isNegative = mayChange = finished = false;
         startStop.IsEnabled = actualTime != null;
         UpdateTimer(actualTime);
      }

      private void StartStop_Click(object sender, RoutedEventArgs e) {
         if (startStopText.Text == Timer.Properties.Resources.start) {
            SetStartStopToStop();
            timer.Start();
         } else {
            SetStartStopToStart();
            timer.Stop();
         }
      }

      private void SetStartStopToStart(bool? disabled = null) {
         startStopIcon.Source = startImg;
         startStopText.Text = Timer.Properties.Resources.start;
         startStop.IsEnabled = disabled ?? actualTime != null;
         useHours.IsEnabled = useMinutes.IsEnabled = true;
      }

      private void SetStartStopToStop() {
         startStopIcon.Source = stopImg;
         startStopText.Text = Timer.Properties.Resources.stop;
         startStop.IsEnabled = actualTime != null;
         useHours.IsEnabled = useMinutes.IsEnabled = false;
      }

      private void UseHours_Checked(object sender, RoutedEventArgs e) {
         if (this.IsLoaded) {
            StartTimeBox_LostFocus(sender, e);
            YellowTimeBox_LostFocus(sender, e);
            RedTimeBox_LostFocus(sender, e);
            if (startTimeBox.BorderBrush == errorBorder) {
               startStop.IsEnabled = false;
               actualTime = null;
            }
            UpdateTimer(actualTime);
         }
      }

      private void StepTimer(object sender, EventArgs e) {
         int actualTime = (int) this.actualTime.Value.TotalSeconds;
         if (mayChange) {
            if (isNegative) {
               ++actualTime;
            } else if (actualTime > 0) {
               --actualTime;
               if (actualTime == 0) {
                  if ((negative.IsChecked ?? false)) {
                     isNegative = true;
                  } else {
                     startStopIcon.Source = startImg;
                     startStopText.Text = Timer.Properties.Resources.start;
                  }
               }
            }
            this.actualTime = new TimeSpan(actualTime * TimeSpan.TicksPerSecond);
            UpdateTimer(this.actualTime);
         } else if ((isNegative || actualTime == 0) && (blinkZero.IsChecked ?? false)) {
            UpdateTimer(null);
         }
         if (actualTime == 0 && !(negative.IsChecked ?? false)) {
            SetStartStopToStart(disabled: true);
            startStop.IsEnabled = false;
         }
         if (playSounds.IsChecked ?? false) {
            Task.Run(() => {
               uint beepCount = 0;
               if (yellowTime != null && this.actualTime == yellowTime) {
                  beepCount = 1;
               } else if (redTime != null && this.actualTime == redTime) {
                  beepCount = 2;
               } else if (actualTime == 0 && !finished) {
                  beepCount = 10;
                  finished = true;
               }
               for (uint i = 0; i < beepCount; ++i) {
                  Console.Beep(500, 250);
                  Thread.Sleep(300);
               }
            });
         }
         mayChange = !mayChange;
      }

      private void UpdateTimer(TimeSpan? actualTime) {
         Brush brush = Brushes.White;
         if ((useRedTime.IsChecked ?? false) && redTime != null && (isNegative || actualTime <= redTime)) {
            brush = Brushes.Red;
         } else if ((useYellowTime.IsChecked ?? false) && yellowTime != null && actualTime <= yellowTime) {
            brush = Brushes.Orange;
         }
         DrawingImage timer = BuildTimer(actualTime, brush, display.ImageWidth);
         display.SetImage(timer);
         mirror.Source = timer;
      }


      private DrawingImage BuildTimer(TimeSpan? time, Brush brush, double width) {
         DrawingGroup draw = new DrawingGroup();
         string value = FormatTimeSpan(time);

         // mm:ss -> 13
         // hh:mm:ss -> 20
         double sepW = width / (useHours.IsChecked ?? false ? 20 : 13);
         double digitW = sepW * 3;
         double height = 118 * sepW / 27;
         double x = 0;

         int max = useHours.IsChecked ?? false ? 8 : 5;
         if (value.Length > max) {
            value = value.Substring(value.Length - max);
         }
         for (int i = 0; i < max; ++i) {
            double w = i == 2 || i == 5 ? sepW : digitW;
            GeometryDrawing gd = new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(x + 2, 2, w - 4, height - 4)));
            ImageDrawing im = new ImageDrawing(GetDigit(value, i), new Rect(x, 0, w, height));
            draw.Children.Add(gd);
            draw.Children.Add(im);
            x += w;
         }
         return new DrawingImage(draw);
      }

      private DrawingImage GetDigit(string value, int index) {
         char digit = value != null && value.Length >= index + 1 ? value[index] : ' ';
         if (index == 2 || index == 5) {
            return (DrawingImage) res[digit == ':' ? "colon" : "colonBlank"];
         } else {
            return (DrawingImage) res["digit" + (digit >= '0' && digit <= '9' ? digit.ToString() : "Blank")];
         }
      }

      private TimeSpan? ParseTimeSpan(string s) {
         if (Regex.Match(s, "^\\d+$").Success) {
            int min = byte.Parse(s);
            return NewTimeSpan(minutes: min);
         } else if (Regex.Match(s, "^\\d+h(\\d+)?$").Success) {
            string[] parts = s.Split('h');
            int hrs = parts.Length > 0 && parts[0].Length > 0 ? int.Parse(parts[0]) : 0;
            int min = parts.Length > 1 && parts[1].Length > 0 ? int.Parse(parts[1]) : 0;
            return NewTimeSpan(hours: hrs, minutes: min);
         } else if (Regex.Match(s, "^\\d+(:\\d+)+$").Success) {
            string[] parts = s.Split(':');
            Array.Reverse(parts);
            int sec = parts.Length > 0 && parts[0].Length > 0 ? int.Parse(parts[0]) : 0;
            int min = parts.Length > 1 && parts[1].Length > 0 ? int.Parse(parts[1]) : 0;
            int hrs = parts.Length > 2 && parts[2].Length > 0 ? int.Parse(parts[2]) : 0;
            return NewTimeSpan(hrs, min, sec);
         } else if (Regex.Match(s, "^\\:\\d+").Success) {
            int sec = int.Parse(s.Substring(1));
            return NewTimeSpan(seconds: sec);
         }
         throw new Exception($"{s} {msg.no_valid_time}");
      }

      private int GetFullMinutes(TimeSpan time) => time.Hours * 60 + time.Minutes;

      private string FormatTimeSpan(TimeSpan? time) {
         if (time is TimeSpan t) {
            return useHours.IsChecked ?? false ? t.ToString() : $"{GetFullMinutes(t):00}:{t.Seconds:00}";
         }
         return "";
      }

      private TimeSpan? NewTimeSpan(int hours = 0, int minutes = 0, int seconds = 0) {
         List<string> result = new List<string>();
         while (seconds >= 60) {
            seconds -= 60;
            minutes += 1;
         }
         if (useHours.IsChecked ?? false) {
            while (minutes >= 60) {
               minutes -= 60;
               hours += 1;
            }
            if (hours > 99) {
               throw new Exception(msg.hours_too_big);
            }
         } else {
            minutes += hours * 60;
            hours = 0;
            if (minutes > 99) {
               throw new Exception(msg.minutes_too_big);
            }
         }
         return new TimeSpan(hours, minutes, seconds);
      }
   }
}