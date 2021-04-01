using Operation.Factory.Interfaces;
using Operation.Factory.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Factory
{
    public class AssembliesManager
    {
        private string _baseWorkDirectory = @"E:\vscode\wp-test";

        public string AssemblyFullPath { get; private set; }
        public string ClassName { get; set; }

        public AssembliesManager(string assemblyPath, string className)
        {
            if (string.IsNullOrEmpty(assemblyPath?.Trim()))
                throw new ArgumentNullException("assemblyPath");

            if (string.IsNullOrEmpty(className?.Trim()))
                throw new ArgumentNullException("className");

            if (Path.IsPathRooted(assemblyPath))
            {
                this.AssemblyFullPath = assemblyPath;
            }
            else
            {
                this.AssemblyFullPath = Path.Combine(_baseWorkDirectory, assemblyPath);
            }
            this.ClassName = className;
        }

        public OperationExecuteResponse Execute(Dictionary<string, string> requestParameters = null)
        {
            ICustomOperation operation = CreateInstanceForOperation();
            this.SetInputParameters(operation, requestParameters);

            OperationResult result = operation.Execute();

            return new OperationExecuteResponse()
            {
                Outputs = this.GetOutputParameters(operation),
                Result = result
            };
        }

        public void SetInputParameters(ICustomOperation customOperation, Dictionary<string, string> requestParameters)
        {
            if (requestParameters == null || requestParameters.Count <= 0) return;

            Type objectType = customOperation.GetType();
            foreach (var parameter in requestParameters)
            {
                PropertyInfo propertyInfo = objectType.GetProperty(parameter.Key);
                if (IsInputParameters(propertyInfo))
                {
                    propertyInfo.SetValue(customOperation, Convert.ChangeType(parameter.Value, propertyInfo.PropertyType));
                }
            }
        }

        private bool IsInputParameters(PropertyInfo property)
        {
            if (property == null) return false;

            object[] attrs = property.GetCustomAttributes(true);
            foreach (object attr in attrs)
            {
                ParameterMappingAttribute paramAttr = attr as ParameterMappingAttribute;
                if (paramAttr != null)
                {
                    return true;
                }
            }
            return false;
        }

        public Dictionary<string, string> GetOutputParameters(ICustomOperation customOperation)
        {
            Dictionary<string, string> outputs = new Dictionary<string, string>();
            foreach (PropertyInfo prop in customOperation.GetType().GetProperties())
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    if (attr is VariableMappingAttribute)
                    {
                        object retObj = prop.GetValue(customOperation);
                        string retStr = retObj != null ? Convert.ToString(retObj) : string.Empty;
                        outputs.Add(prop.Name, retStr);
                    }
                }
            }
            return outputs;
        }

        private Dictionary<string, string> GetOperationParameters(ICustomOperation operation)
        {
            Dictionary<string, string> inputParams = new Dictionary<string, string>();
            foreach (PropertyInfo prop in operation.GetType().GetProperties())
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    ParameterMappingAttribute paramAttr = attr as ParameterMappingAttribute;
                    if (paramAttr != null)
                    {
                        inputParams.Add(prop.Name, paramAttr.Value);
                    }
                }
            }

            return inputParams;
        }

        private ICustomOperation CreateInstanceForOperation()
        {
            Assembly asm = Assembly.LoadFrom(this.AssemblyFullPath);
            Type type = asm.GetType(this.ClassName, true, true);
            return asm.CreateInstance(type.FullName) as ICustomOperation;
        }
    }
}
