﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace km.Collections.MultiZbior
{
    public class MultiSet<T> : IMultiSet<T>
    {
        public MultiSet()
        {
            items = new Dictionary<T, int>();
        }

        private Dictionary<T, int> items;
        private IEqualityComparer<T> comparer;
        public bool IsEmpty => items.Count == 0;
        public IEqualityComparer<T> Comparer => comparer;
        public int Count
        {
            get
            {
                var count = 0;

                foreach(T el in items.Keys)
                {
                    count += items[el];
                }

                return count;
            }
        }
        public bool IsReadOnly => false;
        public MultiSet(IEqualityComparer<T> comparer) : this()
        {
            this.comparer = comparer;
        }
        public MultiSet(IEnumerable<T> sequance) : this()
        {
            foreach(var data in sequance)
            {
                items.Add(data, 1);
            }
        }
        public MultiSet(IEnumerable<T> sequence, IEqualityComparer<T> comparer) : this(sequence)
        {
            this.comparer = comparer;
        }
        public int this[T item] => items[item];


        private void ThrowExceptionIfReadOnly()
        {
            if(IsReadOnly)
                throw new NotSupportedException();
        }

        public static MultiSet<T> operator +(MultiSet<T> first, MultiSet<T> second)
        {
            if(first is null || second is null)
                throw new ArgumentNullException();
            var multiset = new MultiSet<T>();
            var firstEnumerator = first.GetEnumerator();
            var secondEnumerator = second.GetEnumerator();

            while(firstEnumerator.MoveNext())
            {
                multiset.Add(firstEnumerator.Current, first[firstEnumerator.Current]);
            }
            while(secondEnumerator.MoveNext())
            {
                multiset.Add(secondEnumerator.Current, second[secondEnumerator.Current]);
            }
            return multiset;
        }

        public static MultiSet<T> operator -(MultiSet<T> first, MultiSet<T> second)
        {
            if(first is null || second is null)
                throw new ArgumentNullException();

            var multiset = new MultiSet<T>();
            var firstEnumerator = first.GetEnumerator();
            var secondEnumerator = second.GetEnumerator();

            while(firstEnumerator.MoveNext())
            {
                multiset.Add(firstEnumerator.Current, first[firstEnumerator.Current]);
            }
            while(secondEnumerator.MoveNext())
            {
                multiset.Remove(secondEnumerator.Current, second[secondEnumerator.Current]);
            }
            return multiset;
        }

        public static MultiSet<T> operator *(MultiSet<T> first, MultiSet<T> second)
        {
            if(first is null || second is null)
                throw new ArgumentNullException();

            var multiset = new MultiSet<T>();
            var enumerator = first.GetEnumerator();

            while(enumerator.MoveNext()) {
                if(second.Contains(enumerator.Current))
                {
                    if(first[enumerator.Current] >= second[enumerator.Current])
                    {
                        multiset.Add(enumerator.Current, second[enumerator.Current]);
                    }
                    else
                    {
                        multiset.Add(enumerator.Current, first[enumerator.Current]);
                    }
                }
            }
            return multiset;
        }

        public MultiSet<T> Add(T item, int numberOfItems = 1)
        {
            ThrowExceptionIfReadOnly();
            if(numberOfItems > 0)
            {
                if(items.ContainsKey(item))
                {
                    items[item] += numberOfItems;
                }
                else
                {
                    items[item] = numberOfItems;
                }
            }
            return this;
        }

        public void Add(T item) => Add(item, 1);

        public IReadOnlyDictionary<T, int> AsDictionary() => items;

        public IReadOnlySet<T> AsSet()
        {
            var set = new HashSet<T>();
            foreach(T el in items.Keys)
            {
                set.Add(el);
            }
            return set;
        }

        public void Clear() => items.Clear();

        public bool Contains(T item) => items.ContainsKey(item); 

        public void CopyTo(T[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach(T el in items.Keys) {
                for(int j = 0; j < items[el]; j++)
                {
                    array[i] = el;
                    i++;
                }
            }
        }

        public MultiSet<T> ExceptWith(IEnumerable<T> other)
        {
            if(other is null)
            {
                throw new ArgumentNullException();
            }

            ThrowExceptionIfReadOnly();
            var multiset = new MultiSet<T>(other);
            var enumerator = multiset.GetEnumerator();
            while(enumerator.MoveNext()) {
                Remove(enumerator.Current, multiset[enumerator.Current]);
            }

            return this;
        }

        public MultiSet<T> IntersectWith(IEnumerable<T> other)
        {
            if(other is null)
            {
                throw new ArgumentNullException();
            }
            ThrowExceptionIfReadOnly();
            var multiset = new MultiSet<T>(other);
            var enumerator = multiset.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(Contains(enumerator.Current))
                {
                    Remove(enumerator.Current, this[enumerator.Current] - multiset[enumerator.Current]);
                }
                else
                {
                    Remove(enumerator.Current);
                }
            }

            return this;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if(other is null)
            {
                throw new ArgumentNullException();
            }
            if(!IsSubsetOf(other))
                return false;

            var multiset = new MultiSet<T>(other);
            var enumerator = multiset.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(!Contains(enumerator.Current) || multiset[enumerator.Current] > this[enumerator.Current])
                    return true;
            }

            return false;
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if(other is null)
            {
                throw new ArgumentNullException();
            }
            if(!IsSupersetOf(other))
                return false;
            var multiset = new MultiSet<T>(other);
            var enumerator = GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(!multiset.Contains(enumerator.Current) || this[enumerator.Current] > multiset[enumerator.Current])
                    return true;
            }

            return false;
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if(other is null)
            {
                throw new ArgumentNullException();
            }
            var multiset = new MultiSet<T>(other);
            var enumerator = GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(!multiset.Contains(enumerator.Current) || multiset[enumerator.Current] < this[enumerator.Current])
                    return false;
            }

            return true;
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if(other is null)
            {
                throw new ArgumentNullException();
            }
            var multiset = new MultiSet<T>(other);
            var enumerator = multiset.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(!Contains(enumerator.Current) || multiset[enumerator.Current] > this[enumerator.Current])
                    return false;
            }

            return true;
        }

        public bool MultiSetEquals(IEnumerable<T> other)
        {
            var multiset = new MultiSet<T>(other);
            foreach(T el in items.Keys)
            {
                if(!multiset.Contains(el) || multiset[el] != this[el])
                    return false;
            }

            return true;
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if(other is null) {
                throw new ArgumentNullException();
            }
            foreach(T el in other)
            {
                if(Contains(el))
                    return true;
            }

            return false;
        }

        public MultiSet<T> Remove(T item, int numberOfItems = 1)
        {
            ThrowExceptionIfReadOnly();
            if(items.ContainsKey(item) && numberOfItems > 0)
            {
                if(items[item] - numberOfItems <= 0)
                {
                    items.Remove(item);
                }
                else
                {
                    items[item] -= numberOfItems;
                }
            }
            return this;
        }

        public bool Remove(T item) => items.Remove(item);
        public MultiSet<T> RemoveAll(T item)
        {
            ThrowExceptionIfReadOnly();
            Remove(item);
            return this;
        }

        public MultiSet<T> SymmetricExceptWith(IEnumerable<T> other)
        {
            if(other is null)
            {
                throw new ArgumentNullException();
            }
            ThrowExceptionIfReadOnly();
            var multiset = new MultiSet<T>(other);
            var enumerator = multiset.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(Contains(enumerator.Current))
                {
                    Remove(enumerator.Current, multiset[enumerator.Current]);
                }
                else
                {
                    Add(enumerator.Current, multiset[enumerator.Current]);
                }
            }
            return this;
        }

        public MultiSet<T> UnionWith(IEnumerable<T> other)
        {
            if(other is null)
            {
                throw new ArgumentNullException();
            }

            ThrowExceptionIfReadOnly();

            foreach(T el in other)
            {
                Add(el);
            }

            return this;
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(items);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class Enumerator : IEnumerator<T>
        {
            private int index = -1;
            private IReadOnlyDictionary<T, int> elements;

            public Enumerator(IReadOnlyDictionary<T, int> elements)
            {
                this.elements = elements;
            }

            public T Current => elements.ElementAt(index).Key;

            object IEnumerator.Current => Current;

            public void Dispose() {}

            public bool MoveNext()
            {
                if(index + 1 < elements.Count)
                {
                    index++;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                index = (int)default;
            }
        }
    }
}
