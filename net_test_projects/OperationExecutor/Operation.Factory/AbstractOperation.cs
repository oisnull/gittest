using Operation.Factory.Interfaces;
using Operation.Factory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Factory
{
    public abstract class AbstractOperation : ICustomOperation
    {
        public OperationResult Execute()
        {
            try
            {
                return this.ExecuteCore();
            }
            catch (Exception ex)
            {
                return new OperationResult(ex);
            }
        }

        public Task<OperationResult> ExecuteAsync()
        {
            return Task.Factory.StartNew(Execute);
        }

        public abstract OperationResult ExecuteCore();
    }
}
