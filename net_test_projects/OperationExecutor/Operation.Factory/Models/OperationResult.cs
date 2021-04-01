using System;

namespace Operation.Factory.Models
{
    public class OperationResult
    {
        public OperationResultType Type { get; private set; }
        public string Message { get; private set; }

        public OperationResult(OperationResultType type, string message = null)
        {
            this.Type = type;
            this.Message = message;
        }

        public OperationResult(Exception ex)
        {
            if (ex == null || string.IsNullOrEmpty(ex.Message))
            {
                this.Type = OperationResultType.Success;
                this.Message = null;
            }
            else
            {
                this.Type = OperationResultType.Failure;
                this.Message = ex.ToString();
            }
        }
    }

    public enum OperationResultType
    {
        Success = 0,
        Failure = 1,
        //Timeout = 2,
        //Skipped = 3,
        //Canceled = 4,
        //Running = 5
    }
}
