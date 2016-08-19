using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Rendering
{
    public class ParameterMissingException : Exception
    {
        public ParameterMissingException()
            : base()
        {

        }

        public ParameterMissingException(string message)
            : base(message)
        {

        }

        public ParameterMissingException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public ParameterMissingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
