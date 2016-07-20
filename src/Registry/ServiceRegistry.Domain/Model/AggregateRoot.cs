namespace ServiceRegistry.Domain.Model
{
    public abstract class AggregateRoot<TId> : IAggregateRoot<TId>
    {
        public TId Id { get; set; }

        #region Object Member

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var ar = obj as IAggregateRoot<TId>;
            if (ar == null)
                return false;
            return Id.Equals(ar.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion 
    }
}
