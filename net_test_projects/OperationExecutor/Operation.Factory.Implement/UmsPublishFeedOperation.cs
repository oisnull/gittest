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
        [ParameterMapping]
        public string InputParam1 { get; set; }
        [ParameterMapping]
        public string InputParam2 { get; set; }


        [VariableMapping]
        public string OutputParam1 { get; set; }
        [VariableMapping]
        public string OutputParam2 { get; set; }

        public override OperationResult ExecuteCore()
        {
            OutputParam1 = InputParam1;
            OutputParam2 = InputParam2;
            return new OperationResult(OperationResultType.Success, "UmsPublishFeedOperation2");
        }
    }
}
