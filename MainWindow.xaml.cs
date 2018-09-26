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
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Timer {

    public partial class MainWindow : Window {

        private static readonly ResourceDictionary res = Application.Current.Resources;
        private static readonly DrawingImage startImg = (DrawingImage) res["StartImage"];
        private static readonly DrawingImage stopImg = (DrawingImage) res["StopImage"];

        private DisplayWindow display;
        private Time yellowTime = null;
        private Time redTime = null;
        private Time startTime = null;
        private Time actualTime = null;
        private bool mayChange = false;
        private bool isNegative = false;
        private bool finished = false;
        private DispatcherTimer timer;

        public MainWindow() {
            InitializeComponent();
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
            string value = startTimeBox.Text;
            try {
                if (value != null && value != "") {
                    startTime = value;
                    startTimeBox.Text = startTime.ToString();
                } else {
                    startTime = null;
                }
            } catch {
            }
        }

        private void StartTimeBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                StartTimeBox_LostFocus(sender, null);
                SetTimer_Click(sender, null);
                e.Handled = true;
            }
        }

        private void UseYellowTime_Checked(object sender, RoutedEventArgs e) {
            yellowTimeBox.IsEnabled = useYellowTime.IsChecked ?? false;
        }

        private void YellowTimeBox_LostFocus(object sender, RoutedEventArgs e) {
            string value = yellowTimeBox.Text;
            if (value != null && value != "") {
                yellowTime = value;
                yellowTimeBox.Text = yellowTime.ToString();
            } else {
                yellowTime = null;
            }
        }

        private void YellowTimeBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                YellowTimeBox_LostFocus(sender, null);
                e.Handled = true;
            }
        }

        private void UseRedTime_Checked(object sender, RoutedEventArgs e) {
            redTimeBox.IsEnabled = useRedTime.IsChecked ?? false;
        }

        private void RedTimeBox_LostFocus(object sender, RoutedEventArgs e) {
            string value = redTimeBox.Text;
            if (value != null && value != "") {
                redTime = value;
                redTimeBox.Text = redTime.ToString();
            } else {
                redTime = null;
            }
        }

        private void RedTimeBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                RedTimeBox_LostFocus(sender, null);
                e.Handled = true;
            }
        }

        private void SetTimer_Click(object sender, RoutedEventArgs e) {
            if (startStopText.Text == Timer.Properties.Resources.start) {
                timer.Stop();
            }
            actualTime = startTime;
            isNegative = mayChange = finished = false;
            UpdateTimer(actualTime);
        }

        private void StartStop_Click(object sender, RoutedEventArgs e) {
            if (startStopText.Text == Timer.Properties.Resources.start) {
                startStopIcon.Source = stopImg;
                startStopText.Text = Timer.Properties.Resources.stop;
                timer.Start();
            } else {
                startStopIcon.Source = startImg;
                startStopText.Text = Timer.Properties.Resources.start;
                timer.Stop();
            }
        }

        private void StepTimer(object sender, EventArgs e) {
            if (mayChange) {
                if (isNegative) {
                    ++actualTime;
                } else if (actualTime > 0) {
                    --actualTime;
                    if (actualTime == 0) {
                        if ((negative.IsChecked ?? false)) {
                            isNegative = true;
                        } else {
                            startStopIcon.Source = (DrawingImage) res["StartImage"];
                            startStopText.Text = "Start";
                        }
                    }
                }
                UpdateTimer(actualTime);
            } else if ((isNegative || actualTime == 0) && (blinkZero.IsChecked ?? false)) {
                UpdateTimer(null);
            }
            if (playSounds.IsChecked ?? false) {
                Task.Run(() => {
                    uint beepCount = 0;
                    if (yellowTime != null && actualTime == yellowTime) {
                        beepCount = 1;
                    } else if (redTime != null && actualTime == redTime) {
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

        private void UpdateTimer(Time actualTime) {
            startStop.IsEnabled = actualTime != null && actualTime > 0;
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


        private DrawingImage BuildTimer(Time time, Brush brush, double width) {
            DrawingGroup draw = new DrawingGroup();
            string value = time != null ? time.ToString() : "";
            double r = width / 13;
            double w = r * 3;
            double h = 118 * r / 27;
            double x = 0;


            GeometryDrawing gd;
            ImageDrawing im;

            gd = new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(x + 2, 2, w - 4, h - 4)));
            im = new ImageDrawing(GetDigit(value, 0), new Rect(x, 0, w, h));
            draw.Children.Add(gd);
            draw.Children.Add(im);
            x += w;

            gd = new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(x + 2, 2, w - 4, h - 4)));
            im = new ImageDrawing(GetDigit(value, 1), new Rect(x, 0, w, h));
            draw.Children.Add(gd);
            draw.Children.Add(im);
            x += w;

            gd = new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(x + 2, 2, r - 4, h - 4)));
            im = new ImageDrawing(GetDigit(value, 2), new Rect(x, 0, r, h));
            draw.Children.Add(gd);
            draw.Children.Add(im);
            x += r;

            gd = new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(x + 2, 2, w - 4, h - 4)));
            im = new ImageDrawing(GetDigit(value, 3), new Rect(x, 0, w, h));
            draw.Children.Add(gd);
            draw.Children.Add(im);
            x += w;

            gd = new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(x + 2, 2, w - 4, h - 4)));
            im = new ImageDrawing(GetDigit(value, 4), new Rect(x, 0, w, h));
            draw.Children.Add(gd);
            draw.Children.Add(im);

            return new DrawingImage(draw);
        }

        private DrawingImage GetDigit(string value, int index) {
            char digit = value != null && value.Length >= index + 1 ? value[index] : ' ';
            if (index == 2) {
                return (DrawingImage) res[digit == ':' ? "colon" : "colonBlank"];
            } else {
                return (DrawingImage) res["digit" + (digit >= '0' && digit <= '9' ? digit.ToString() : "Blank")];
            }
        }
    }
}
