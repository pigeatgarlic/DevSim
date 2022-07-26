using System.Threading.Tasks;


namespace DevSim.Interfaces
{    
    public interface IDtoMessageHandler
    {
        Task ParseMessage(byte[] message);
    }
}