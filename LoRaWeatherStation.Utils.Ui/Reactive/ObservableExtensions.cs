using System;
using System.Reactive.Linq;

namespace LoRaWeatherStation.Utils.Reactive
{
    public static class ObservableExtensions
    {
        public static IObservable<TResult> Select<T1, T2, TResult>(this IObservable<(T1, T2)> source, Func<T1, T2, TResult> selector)
            => source.Select(t => selector(t.Item1, t.Item2));
        public static IObservable<TResult> Select<T1, T2, T3, TResult>(this IObservable<(T1, T2, T3)> source, Func<T1, T2, T3, TResult> selector)
            => source.Select(t => selector(t.Item1, t.Item2, t.Item3));
        public static IObservable<TResult> Select<T1, T2, T3, T4, TResult>(this IObservable<(T1, T2, T3, T4)> source, Func<T1, T2, T3, T4, TResult> selector)
            => source.Select(t => selector(t.Item1, t.Item2, t.Item3, t.Item4));
        public static IObservable<TResult> Select<T1, T2, T3, T4, T5, TResult>(this IObservable<(T1, T2, T3, T4, T5)> source, Func<T1, T2, T3, T4, T5, TResult> selector)
            => source.Select(t => selector(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5));
        public static IObservable<TResult> Select<T1, T2, T3, T4, T5, T6, TResult>(this IObservable<(T1, T2, T3, T4, T5, T6)> source, Func<T1, T2, T3, T4, T5, T6, TResult> selector)
            => source.Select(t => selector(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6));
        public static IObservable<TResult> Select<T1, T2, T3, T4, T5, T6, T7, TResult>(this IObservable<(T1, T2, T3, T4, T5, T6, T7)> source, Func<T1, T2, T3, T4, T5, T6, T7, TResult> selector)
            => source.Select(t => selector(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7));
        public static IObservable<TResult> Select<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this IObservable<(T1, T2, T3, T4, T5, T6, T7, T8)> source, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> selector)
            => source.Select(t => selector(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8));
        public static IObservable<TResult> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this IObservable<(T1, T2, T3, T4, T5, T6, T7, T8, T9)> source, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> selector)
            => source.Select(t => selector(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8, t.Item9));
    }
}