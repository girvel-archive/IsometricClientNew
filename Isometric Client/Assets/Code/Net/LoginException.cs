using System;
using System.Runtime.Serialization;

namespace Assets.Code.Net
{
    [Serializable]
    public class LoginException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public LoginException()
        {
        }

        public LoginException(string message) : base(message)
        {
        }

        public LoginException(string message, Exception inner) : base(message, inner)
        {
        }

        protected LoginException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}