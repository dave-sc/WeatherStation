using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

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
        
        public static IObservable<TResult> Select<TObservable, TResult>(this IObservable<TObservable> observable, Func<Task<TResult>> selector)
        {
            return observable.Select(_ => Observable.FromAsync(selector))
                .Concat();
        }
        
        public static IObservable<TResult> Select<TObservable, TResult>(this IObservable<TObservable> observable, Func<TObservable, Task<TResult>> selector)
        {
            return observable.Select(x => Observable.FromAsync(() => selector(x)))
                .Concat();
        }
        
        //https://stackoverflow.com/a/11464537
        public static IObservable<T> RepeatLastWhen<T>(this IObservable<T> inner, TimeSpan period)
        {
            return inner.RepeatLastWhen(Observable.Interval(period));
        }
        
        //https://stackoverflow.com/a/11464537
        public static IObservable<TResult> RepeatLastWhen<TResult, TDontCare>(this IObservable<TResult> inner, IObservable<TDontCare> trigger)
        {    
            return inner.Select(x => 
                trigger
                    .Select(_ => x)
                    .StartWith(x)
            ).Switch();
        }

        public static IObservable<(T1, T2)> CombineLatest<T1, T2>(this IObservable<T1> first, IObservable<T2> second) =>
            first.CombineLatest(second, (a, b) => (a, b));
        public static IObservable<(T1, T2, T3)> CombineLatest<T1, T2, T3>(this IObservable<T1> first, IObservable<T2> second, IObservable<T3> third) =>
            first.CombineLatest(second, third, (a, b, c) => (a, b, c));
        public static IObservable<(T1, T2, T3, T4)> CombineLatest<T1, T2, T3, T4>(this IObservable<T1> first, IObservable<T2> second, IObservable<T3> third, IObservable<T4> forth) =>
            first.CombineLatest(second, third, forth, (a, b, c, d) => (a, b, c, d));
        public static IObservable<(T1, T2, T3, T4, T5)> CombineLatest<T1, T2, T3, T4, T5>(this IObservable<T1> first, IObservable<T2> second, IObservable<T3> third, IObservable<T4> forth, IObservable<T5> fifth) =>
            first.CombineLatest(second, third, forth, fifth, (a, b, c, d, e) => (a, b, c, d, e));
    }
}