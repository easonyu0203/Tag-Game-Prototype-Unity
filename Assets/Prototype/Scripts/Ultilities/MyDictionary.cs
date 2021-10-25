using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ultilities
{
    
    public class MyDictionary<TKey, TValue>: IEnumerable<KeyValuePair<TKey, TValue>>{
        private Dictionary<TKey, TValue> _dic = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Invoke right after the dictionary change
        /// </summary>
        public event Action OnDicChange;

        public TValue this[TKey key] { get {
            return _dic[key];
        }}
        public void Add(TKey key, TValue value){
            _dic.Add(key, value);
            OnDicChange?.Invoke();
        }

        public bool Remove(TKey key){
            if(_dic.Remove(key)){
                OnDicChange?.Invoke();
                return true;
            }
            else{
                return false;
            }
            
        }

        public bool TryGetValue(TKey key, out TValue value){
            return _dic.TryGetValue(key, out value);
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
