using Operation.Factory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Factory.Implement
{
    public class UmsPublishFeedOperation : AbstractOperation
    {
        public override OperationResult ExecuteCore()
        {
            return new OperationResult(OperationResultType.Success, "UmsPublishFeedOperation");
        }
    }
}
