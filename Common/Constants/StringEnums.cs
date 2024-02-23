using System;
namespace Common.Constants
{
    public static class StringEnums
    {
        private class StringValue : Attribute
        {
            public string Value { get; }
            public StringValue(string value)
            {
                Value = value;
            }
        }
    
        public static string ToValue(Enum thisEnum)
        {
            string output = null;
            var type = thisEnum.GetType();

            var fieldInfo = type.GetField(thisEnum.ToString());
            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(StringValue), false) as StringValue[];
                if (attrs!= null && attrs.Length > 0)
                {
                    output = attrs[0].Value;
                }
            }

            return output;
        }
    
        public enum Roles
        {
            [StringValue("User")] UserRole,
            [StringValue("Supplier")] SupplierRole,
            [StringValue("Admin")] AdminRole,
        }
    }
}