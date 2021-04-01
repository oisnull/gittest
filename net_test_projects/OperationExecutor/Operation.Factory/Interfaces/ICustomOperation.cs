using Operation.Factory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Factory.Interfaces
{
    public interface ICustomOperation
    {
        OperationResult Execute();
        Task<OperationResult> ExecuteAsync();
    }
}
