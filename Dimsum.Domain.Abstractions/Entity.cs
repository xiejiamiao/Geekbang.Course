﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DimSum.Domain.Abstractions
{
    public abstract class Entity:IEntity
    {
        public abstract object[] GetKeys();

        public override string ToString()
        {
            return $"[Entity:{GetType().Name}]  Keys={string.Join(",", GetKeys())}";
        }

        #region Transaction

        private List<IDomainEvent> _domainEvents;

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents ??= new List<IDomainEvent>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvent()
        {
            _domainEvents?.Clear();
        }


        #endregion
    }

    public abstract class Entity<TKey> : Entity, IEntity<TKey>
    {
        int? _requestedHashCode;
        public virtual TKey Id { get; protected set; }

        public override object[] GetKeys()
        {
            return new object[] {Id};
        }

        public bool IsTransient()
        {
            return EqualityComparer<TKey>.Default.Equals(Id, default);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity<TKey>))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity<TKey> item = (Entity<TKey>)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id.Equals(this.Id);
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31;

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Entity:{GetType().Name}] Id = {Id}";
        }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }
    }
}
