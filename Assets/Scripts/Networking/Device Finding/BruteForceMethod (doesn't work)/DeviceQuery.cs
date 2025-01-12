// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)


using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

/// <summary>
/// This class handles a single query to a single device and checks whether this device gets a response or not.
/// </summary>
class DeviceQuery
{
    private string target;
    private int    attempts;
    private int    timeoutMiliseconds;
    
    private List<Task> currentTasks    = new List<Task>();
    private bool       success         = false;
    private int        finishedCounter = 0;
    
    public bool Finished { get; private set; } = false;
    
    public DeviceQuery(string target, int attempts, int timeoutMiliseconds)
    {
        this.target = target;
        this.attempts = attempts;
        this.timeoutMiliseconds = timeoutMiliseconds;
    }
    
    public Task<bool> HasConnection()
    {
        for (int i = 0; i < attempts; i++)
            currentTasks.Add(new Ping().SendPingAsync(target, timeoutMiliseconds)
                .ContinueWith(t => ReportResponse(t, i)));
        
        return new Task<bool>(() =>
        {
            while (!Finished)
            {
            }
            return success;
        });
    }
    
    private void ReportResponse(Task<PingReply> reply, int attempt)
    {
        //check if the ping was a success
        if (reply.Result.Status == 0)
        {
            success = true;
            Finished = true;
            
            //cancel all remaining tasks
        }
        else
        {
            finishedCounter++;
            if (finishedCounter == currentTasks.Count)
                Finished = true;
        }
    }
}