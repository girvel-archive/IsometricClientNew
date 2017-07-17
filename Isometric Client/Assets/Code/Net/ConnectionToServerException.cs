using System;
using System.Runtime.Serialization;

namespace Assets.Code.Net
{
    [Serializable]
    public class ConnectionToServerException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ConnectionToServerException()
        {
        }

        public ConnectionToServerException(string message) : base(message)
        {
        }

        public ConnectionToServerException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ConnectionToServerException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}