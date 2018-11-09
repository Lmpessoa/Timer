![Timer main screen](https://github.com/lmpessoa/timer/raw/images/main-screenshot.png)

This is a simple application to provide a timer for events. 

Application settings are very simple as can be seen from the interface. You can set whatever time you need for start and hit play to start counting down. Currently the timer is limited to minutes and seconds (thus a practical limit of about 100 minutes - less one secont).

## Alerts

Options on the timer allow you to notify people present at the event with visual and auditive cues at a middle time (not actually bound to the middle of the start time), after when the timer will be shown in yellow, and a final time, after when the timer will be shown in red (colours cannot be changed). If the option is selected, the application will play a discrete sound at the given alert times.

![Timer alert options](https://github.com/lmpessoa/timer/raw/images/alerts-screenshot.png)

## Other options

It is also possible to make the timer blink once it reaches zero and to continue counting after the timer expired. However, continuing the counting after zero makes it become upward thus it is recommended to either use blinking and/or final alert (it works even set to 00:00) to distinguish the expired time.