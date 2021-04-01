using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Factory.Models
{
    public class ParameterMappingAttribute : Attribute
    {
        public ParameterMappingAttribute()
        {

        }
        public ParameterMappingAttribute(string value)
        {
            this.Value = value;
        }
        public string Value { get; set; }
    }
}
