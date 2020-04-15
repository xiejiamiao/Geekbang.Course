using System;
using System.Collections.Generic;

namespace Dimsum.Domain.Abstractions
{
    public abstract class Entity : IEntity
    {
        public abstract object[] GetKeys();

        public override string ToString()
        {
            return $"[Entity:{GetType().Name}] Keys={string.Join(",", GetKeys())}";
        }

        private List<IDomainEvent> _domainEvents;
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents ??= new List<IDomainEvent>();
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents?.Remove(domainEvent);
        }

        public void ClearDomainEvent()
        {
            _domainEvents?.Clear();
        }

    }

    public abstract class Entity<TKey> : Entity, IEntity<TKey>
    {
        private int? _requestHashCode;

        public virtual TKey Id { get; protected set; }

        public override object[] GetKeys()
        {
            return new object[] {Id};
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TKey>))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            Entity<TKey> item = (Entity<TKey>) obj;
            if (item.IsTransient() || this.IsTransient())
            {
                return false;
            }
            else
            {
                return item.Id.Equals(this.Id);
            }
        }


        public bool IsTransient()
        {
            return EqualityComparer<TKey>.Default.Equals(Id, default);
        }

        public override string ToString()
        {
            return $"[Entity: {GetType().Name}] Id={Id}";
        }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        {
            if (Object.ReferenceEquals(left, null))
            {
                return (object.ReferenceEquals(right, null)) ? true : false;
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }
    }
}
