using Logictracker.Model.EnumTypes;

namespace Logictracker.Model.Utils
{
    public class BackwardReply
    {
        public ReplyAction Action { get; set; }

        public BackwardReply()
        {
            Action = ReplyAction.None;
        }

        public BackwardReply(ReplyAction a)
        {
            Action = a;
        }

        public static BackwardReply NullIfNothing(BackwardReply reply)
        {
            if (reply == null || reply.Action == ReplyAction.None) return null;
            return reply;
        }

        public static bool IsNothing(BackwardReply reply)
        {
            return reply == null || reply.Action == ReplyAction.None;
        }

        public static bool Is(BackwardReply reply, ReplyAction action)
        {
            return reply != null && reply.Action == action;
        }

        public static BackwardReply None 
        { 
            get {
                return new BackwardReply(ReplyAction.None);   
            }
        }

        public static BackwardReply ReturnedResponse
        {
            get
            {
                return new BackwardReply(ReplyAction.ReturnedResponse);
            }
        }

        public static BackwardReply Release
        {
            get
            {
                return new BackwardReply(ReplyAction.Release);
            }
        }

        public static BackwardReply ReleaseSilently
        {
            get
            {
                return new BackwardReply(ReplyAction.ReleaseSilently);
            }
        }

        public static BackwardReply NotSupported
        {
            get
            {
                return new BackwardReply(ReplyAction.NotSupported);
            }
        }

    };
}