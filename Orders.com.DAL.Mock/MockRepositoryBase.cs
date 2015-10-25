﻿using AutoMapper;
using Orders.com.Core.DataProxy;
using Orders.com.Core.Domain;
using Peasy;
using Peasy.Core.Extensions;
using Peasy.Core;
using Peasy.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Peasy.Attributes;

namespace Orders.com.DAL.Mock
{
    public abstract class MockRepositoryBase<DTO, TKey> : IServiceDataProxy<DTO, TKey> where DTO : IDomainObject<TKey>
    {
        private object _lockObject = new object();

        public MockRepositoryBase()
        {
            Mapper.CreateMap<DTO, DTO>();
        }

        private ConcurrentDictionary<TKey, DTO> _data;

        protected ConcurrentDictionary<TKey, DTO> Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new ConcurrentDictionary<TKey, DTO>();
                    SeedRepository().ForEach(item =>
                    {
                        var rule = item.ID.CreateValueRequiredRule("ID").Validate();
                        if (!rule.IsValid || item.ID == null)
                            throw new ArgumentException($"All values for {item.GetType().Name}.ID must be supplied");
                        _data[item.ID] = item;
                    });
                }
                return _data;
            }
        }

        protected virtual IEnumerable<DTO> SeedRepository()
        {
            return Enumerable.Empty<DTO>();
        }

        protected abstract TKey GetNextID();

        public virtual IEnumerable<DTO> GetAll()
        {
            Debug.WriteLine($"Executing {this.GetType().Name}.GetAll");
            return Data.Values.Select(Mapper.Map<DTO, DTO>).ToArray();
        }

        public virtual DTO GetByID(TKey id)
        {
            Debug.WriteLine($"Executing {this.GetType().Name}.GetByID");
            var item = Data[id];
            return Mapper.Map(item, default(DTO));
        }

        public virtual DTO Insert(DTO entity)
        {
            //lock (_lockObject)
            //{
            Debug.WriteLine($"Executing {this.GetType().Name}.Insert");
            var nextID = GetNextID();
            entity.ID = nextID;
            Data[nextID] = Mapper.Map(entity, default(DTO));
            return entity;
            //}
        }

        public virtual DTO Update(DTO entity)
        {
            Debug.WriteLine($"Executing {this.GetType().Name}.Update");
            var existing = Data[entity.ID];
            Mapper.Map(entity, existing);
            return entity;
        }

        public virtual void Delete(TKey id)
        {
            Debug.WriteLine($"Executing {this.GetType().Name}.Delete");
            var entity = default(DTO);
            Data.TryRemove(id, out entity);
        }

        public virtual async Task<IEnumerable<DTO>> GetAllAsync()
        {
            return GetAll();
        }

        public virtual async Task<DTO> GetByIDAsync(TKey id)
        {
            return GetByID(id);
        }

        public virtual async Task<DTO> InsertAsync(DTO entity)
        {
            return Insert(entity);
        }

        public virtual async Task<DTO> UpdateAsync(DTO entity)
        {
            return Update(entity);
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            Delete(id);
        }

        public virtual bool SupportsTransactions
        {
            get { return false; }
        }

        public virtual bool IsLatencyProne
        {
            get { return false; }
        }
    }
}
