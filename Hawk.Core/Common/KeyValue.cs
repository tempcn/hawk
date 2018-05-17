using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hawk.Common
{
    public class KeyValue<TKey, TValue> 
        //where TKey : struct
        //where TValue: class
    {
        private KeyValuePair<TKey, TValue> pair = new KeyValuePair<TKey, TValue>();

        public Object Data { get; set; }

        public TKey Key
        {
            get { return pair.Key; }
            set { pair = new KeyValuePair<TKey, TValue>(value, pair.Value); }
        }

        public TValue Value
        {
            get { return pair.Value; }
            set { pair = new KeyValuePair<TKey, TValue>(pair.Key, value); }
        }

        public void SetKeyValue(TKey key, TValue value)
        {
            pair = new KeyValuePair<TKey, TValue>(key, value);
        }

        public void SetKeyValue(TKey key, TValue value, Object data)
        {
            pair = new KeyValuePair<TKey, TValue>(key, value);
            this.Data = data;
        }

        public KeyValue(TKey key, TValue value) { SetKeyValue(key, value); }

        public KeyValue(TKey key, TValue value, Object data) { SetKeyValue(key, value, data); }
    }
}