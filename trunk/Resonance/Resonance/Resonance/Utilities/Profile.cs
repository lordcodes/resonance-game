using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Resonance
{
    public class Profile
    {
        private static String result = "";
        private static int req = 0;

        internal Profile(string n)
        {
            name_ = n;
            binLevels_[0] = Stopwatch.Frequency / 10000;
            binLevels_[1] = Stopwatch.Frequency / 1000;
            binLevels_[2] = Stopwatch.Frequency / 100;
            binLevels_[3] = Stopwatch.Frequency / 10;
        }

        internal void Reset()
        {
            time_ = 0;
            count_ = 0;
            maximum_ = 0;
            bins_ = new int[5];
        }

        internal string name_;
        internal long time_;
        internal long count_;
        internal long maximum_;
        internal int[] bins_ = new int[5];
        static internal long[] binLevels_ = new long[4];
        int nestLevel_;
        long startTime_;

        public void Enter()
        {
            if (nestLevel_ == 0)
                startTime_ = Stopwatch.GetTimestamp();
            ++nestLevel_;
        }

        public void Exit()
        {
            --nestLevel_;
            if (nestLevel_ == 0)
            {
                ++count_;
                long l = Stopwatch.GetTimestamp() - startTime_;
                time_ += l;
                if (l > maximum_)
                    maximum_ = l;
                int i = 0;
                while (i < 4)
                {
                    if (binLevels_[i] > l)
                        break;
                    ++i;
                }
                bins_[i]++;
            }
            Debug.Assert(nestLevel_ >= 0);
        }

        internal class Foo : IDisposable
        {
            static Foo foos_ = new Foo();
            Foo next_;
            Profile p_;

            Foo()
            {
            }

            Foo(Profile p)
            {
                p_ = p;
                p.Enter();
            }
            public void Dispose()
            {
                if (p_ != null)
                {
                    Debug.Assert(next_ == null);
                    p_.Exit();
                    p_ = null;
                    next_ = foos_;
                    foos_ = this;
                }
            }

            ~Foo()
            {
                Debug.Assert(p_ == null);
            }

            internal static Foo NewFoo(Profile p)
            {
                if (foos_ == null)
                    return new Foo(p);
                Foo ret = foos_;
                foos_ = ret.next_;
                ret.next_ = null;
                ret.p_ = p;
                p.Enter();
                return ret;
            }
        }

        //  you can use using() to measure time around a specific area
        //  and not have to worry about exceptions. This is fairly efficient, 
        //  and doesn't actually generate any garbage.
        public IDisposable Measure()
        {
            return Foo.NewFoo(this);
        }

        static Dictionary<string, Profile> profiles_ = new Dictionary<string, Profile>();

        public static Profile Get(string name)
        {
            Profile ret;
            if (profiles_.TryGetValue(name, out ret))
                return ret;
            ret = new Profile(name);
            profiles_.Add(name, ret);
            return ret;
        }

        public static String Dump()
        {
            return Dump(false);
        }

        public static String DumpAndReset()
        {
            return Dump(true);
        }

        internal static String Dump(bool andReset)
        {
            req++;
            if (req > 60)
            {
                req = 0;
                result = "";
                result += "--------------- Profile dump: ---------------\n";
                result += "Profiler, ms total, count, ms max\n";
                result += "      Histogram (<0.1, <1.0, <10.0, <100.0, ++ ms)\n";
                double freq = (double)Stopwatch.Frequency;
                foreach (Profile p in profiles_.Values)
                {
                    result += String.Format("{0}, {1}, {2}, {3}", p.name_, 1000.0 * p.time_ / freq, p.count_,
                        1000.0 * p.maximum_ / freq) + "\n";
                    result += String.Format("      {0} {1} {2} {3} {4}", p.bins_[0], p.bins_[1], p.bins_[2], p.bins_[3], p.bins_[4]) + "\n";
                    if (andReset) p.Reset();
                }
                result += "--------------- End profile ---------------\n";
            }
            return result;
        }
    }
}
