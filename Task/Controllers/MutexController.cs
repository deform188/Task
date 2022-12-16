using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Threading;

namespace Task.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MutexController : ControllerBase
    {
        public static SemaphoreSlim semaphore =  new SemaphoreSlim(1,1);
        public static ConcurrentQueue<string> cq = new ConcurrentQueue<string>();

        public MutexController()
        {
            semaphore.WaitAsync().ContinueWith(t =>
            {
                while (cq.TryPeek(out var peek))
                { 
                
                    //Some work with peek
                    Thread.Sleep(5000);
                }
            });
        }

        [HttpPost("enter")]
        public IActionResult Enter(string userId)
        {
            cq.Enqueue(userId);

            if (cq.Count == 1)
            {
                semaphore.Release();
            }

            return Ok($"User {userId} get resource");
        }

        [HttpGet("exit")]
        public IActionResult Exit(string userId)
        {

            if (cq.TryPeek(out var peek))
            {
                if (peek == userId)
                {
                    cq.TryDequeue(out var result);
                    return Ok($"User {userId} return resource");
                }
            }

            return BadRequest();
        }
    }
}