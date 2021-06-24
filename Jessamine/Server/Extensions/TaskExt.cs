using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jessamine.Server.Extensions
{
  public class TaskExt
  {
    public static async Task<IEnumerable<T>> WhenAll<T>(params Task<T>[] tasks)
    {
      var allTasks = Task.WhenAll(tasks);

      try
      {
        return await allTasks;
      }
      catch (Exception)
      {
        // ignore
      }

      throw allTasks.Exception ?? throw new Exception("Something went really wrong");
    }

    public static async Task WhenAll(params Task[] tasks)
    {
      var allTasks = Task.WhenAll(tasks);

      try
      {
        await allTasks;
        return;
      }
      catch (Exception)
      {
        // ignore
      }

      throw allTasks.Exception ?? throw new Exception("Something went really wrong");
    }
  }
}
