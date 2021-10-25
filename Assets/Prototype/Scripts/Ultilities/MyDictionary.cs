using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ultilities
{
    
    public class MyDictionary<TKey, TValue>: IEnumerable<KeyValuePair<TKey, TValue>>{
        private Dictionary<TKey, TValue> _dic = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Invoke right before Add
        /// </summary>
        public event Action<TKey, TValue> OnAdd;
        /// <summary>
        /// Invoke right before Remove
        /// </summary>
        public event Action<TKey, TValue> OnRemove;

        public TValue this[TKey key] { get {
            return _dic[key];
        }}
        public void Add(TKey key, TValue value){
            OnAdd?.Invoke(key, value);
            _dic.Add(key, value);
        }

        public bool Remove(TKey key){
            if(_dic.ContainsKey(key)){
                OnRemove?.Invoke(key, _dic[key]);
                _dic.Remove(key);
                return true;
            }
            else{
                return false;
            }
            
        }

        public bool TryGetValue(TKey key, out TValue value){
            return _dic.TryGetValue(key, out value);
        }

        public bool ContainsValue(TValue v)
        {
            return _dic.ContainsValue(v);
        }

        public bool ContainsKey(TKey k)
        {
            return _dic.ContainsKey(k);
        }

        public int Count(){
            return _dic.Count;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dic.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dic.GetEnumerator();
        }
    }

}
