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

namespace Timer {

    public sealed class Time {

        private uint value;

        public Time(uint value) {
            this.value = value;
        }

        public Time(uint minutes, uint seconds) {
            this.value = minutes * 60 + seconds;
        }

        public static Time operator ++(Time time) {
            return new Time(time.value + 1);
        }

        public static Time operator --(Time time) {
            return new Time(time.value - 1);
        }

        public static bool operator ==(Time t1, Time t2) {
            return Compare(t1, t2) == 0;
        }

        public static bool operator !=(Time t1, Time t2) {
            return Compare(t1, t2) != 0;
        }

        public static bool operator <(Time t1, Time t2) {
            return Compare(t1, t2) > 0;
        }

        public static bool operator >(Time t1, Time t2) {
            return Compare(t1, t2) < 0;
        }

        public static bool operator <=(Time t1, Time t2) {
            return Compare(t1, t2) >= 0;
        }

        public static bool operator >=(Time t1, Time t2) {
            return Compare(t1, t2) <= 0;
        }

        private static int Compare(Time t1, Time t2) {
            object o1 = t1;
            object o2 = t2;
            if (o1 == null && o2 == null) {
                return 0;
            } else if (o1 == null) {
                return 1;
            } else if (o2 == null) {
                return -1;
            }
            return (int) t2.value - (int) t1.value;
        }

        public static implicit operator Time(uint i) {
            return new Time(i);
        }

        public static implicit operator Time(string s) {
            string[] parts = s.Split(':');
            uint min = parts.Length > 0 && parts[0].Length > 0 ? uint.Parse(parts[0]) : 0;
            uint sec = parts.Length > 1 && parts[1].Length > 0 ? uint.Parse(parts[1]) : 0;
            return new Time(min, sec);
        }

        public override bool Equals(object obj) {
            if (obj is Time) {
                return this == (Time) obj;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return (int) value;
        }

        public override string ToString() {
            uint sec = value % 60;
            uint min = (value - sec) / 60;
            string result = "";
            if (min < 10) {
                result += "0";
            }
            result += min;
            result += ":";
            if (sec < 10) {
                result += "0";
            }
            result += sec;
            return result;
        }
    }
}
