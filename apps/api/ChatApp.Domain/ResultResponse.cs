using ChatApp.Domain.Enum;

namespace ChatApp.Domain
{
    public class ResultResponse<T>
    {
        public ResultResponse(T data) : this(data, MessageType.None, string.Empty)
        {
        }

        public ResultResponse(T data, MessageType messageType) : this(data, messageType, string.Empty)
        {
        }

        public ResultResponse(T data, MessageType messageType, string message)
        {
            Data = data;
            MessageType = messageType;
            Message = message;
        }

        public T Data { get; set; }
        public string Message { get; set; }
        public MessageType MessageType { get; set; }
    }
}
