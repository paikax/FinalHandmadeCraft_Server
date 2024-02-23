using System;
using System.Linq.Expressions;

namespace Data.Entities.User
{
    public class UpdateManyEntitiesParams<TEntity, TField> where TEntity : BaseEntity
    {
        public Expression<Func<TEntity, TField>> Field { get; set; }
        public TField Value { get; set; }
    }
}