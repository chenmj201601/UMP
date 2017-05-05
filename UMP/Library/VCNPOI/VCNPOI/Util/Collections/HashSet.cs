//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    5f2c9369-b041-41f1-93d3-4a8bf26a2b6d
//        CLR Version:              4.0.30319.42000
//        Name:                     HashSet
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.Util.Collections
//        File Name:                HashSet
//
//        Created by Charley at 2016/9/30 16:20:09
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections;
using System.Collections.Generic;


namespace VoiceCyber.NPOI.Util.Collections
{
    /// <summary>
    /// This class comes from Java
    /// </summary>
    public class HashSet<T> : ICollection<T>
    {
        private readonly Dictionary<T, object> impl = new Dictionary<T, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet"/> class.
        /// </summary>
        public HashSet()
        {
        }

        /// <summary>
        /// Adds the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        public void Add(T o)
        {
            if (IsReadOnly)
                throw new InvalidOperationException("this hashset is readonly");
            impl[o] = null;
        }

        /// <summary>
        /// Determines whether [contains] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified o]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T o)
        {
            if (o == null)
                return false;
            return impl.ContainsKey(o);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is less than zero.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(T[] array, int index)
        {
            impl.Keys.CopyTo(array, index);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        public int Count
        {
            get { return impl.Count; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return impl.Keys.GetEnumerator();
        }

        public bool IsReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Removes the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        public bool Remove(T o)
        {
            if (IsReadOnly)
                throw new InvalidOperationException("this hashset is readonly");
            impl.Remove(o);
            return true;
        }


        /// <summary>
        /// Removes all of the elements from this set.
        /// The set will be empty after this call returns.
        /// </summary>
        public void Clear()
        {
            impl.Clear();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return impl.GetEnumerator();
        }
    }
}
