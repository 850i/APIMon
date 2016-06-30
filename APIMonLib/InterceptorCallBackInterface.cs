
namespace APIMonLib
{
    public interface InterceptorCallBackInterface
    {
        void dataHasBeenIntercepted(TransferUnit tu);
        //void messageHasBeenQueued(TransferUnit tu);
    }
}
