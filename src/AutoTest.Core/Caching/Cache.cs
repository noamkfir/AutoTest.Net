﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.DebugLog;

namespace AutoTest.Core.Caching
{
    class Cache : ICache
    {
        private IServiceLocator _services;
        private List<IRecord> _records = new List<IRecord>();

        public int Count { get { return _records.Count; } }

        public Cache(IServiceLocator services)
        {
            _services = services;
        }

        public void Add<T>(string key) where T : IRecord
        {
            var index = findIndex(key);
            if (index < 0)
                index = createRecord<T>(key);
            var record = _records[index];
            if (!prepareRecord<T>(index))
                _records.Remove(record);
        }

        public bool Exists(string key)
        {
            return findIndex(key) >= 0;
        }

        public T Get<T>(string key) where T : IRecord
        {
            return Get<T>(findIndex(key));
        }

        public T Get<T>(int index) where T : IRecord
        {
            if (index < 0)
                return default(T);
            return (T) _records[index];
        }

        public T[] GetAll<T>() where T : IRecord
        {
            var unpreparedRecords = _records.Where(x => x.GetType().Equals(typeof(T)));
            var records = new List<T>();
            foreach (var record in unpreparedRecords)
                records.Add((T) record);
            return records.ToArray();
        }

        public void Reload<T>(string key) where T : IRecord
        {
            var index = findIndex(key);
            if (index < 0)
                return;
            var dirtyMarker = _services.Locate<IReload<T>>();
            T record = (T) _records[index];
            dirtyMarker.MarkAsDirty(record);
            prepareRecord<T>(index);
        }

        private int createRecord<T>(string key) where T : IRecord
        {
			Debug.WriteMessage(string.Format("{0} initialize cache", key));
            var creator = _services.Locate<ICreate<T>>();
            _records.Add(creator.Create(key));
            int index = _records.Count - 1;
            return index;
        }

        private bool prepareRecord<T>(int index) where T : IRecord
        {
            T record = (T)_records[index];
			Debug.WriteMessage(string.Format("{0} preparing cache", record.Key));
            var preparer = _services.Locate<IPrepare<T>>();
            T preparedRecord = preparer.Prepare(record);
            if (preparedRecord !=  null)
                _records[index] = preparedRecord;
            return preparedRecord != null;
        }

        private void addInternal<T>(T r)
        {
            _records.Add((IRecord) r);
            if (!prepareRecord<IRecord>(_records.Count - 1))
                _records.Remove((IRecord) r);
        }

        private int findIndex(string key)
        {
            return _records.FindIndex(0, p => p.Key.Equals(key));
        }
    }
}
