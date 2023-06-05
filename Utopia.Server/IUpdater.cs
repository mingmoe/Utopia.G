using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Server;

/// <summary>
/// 更新器
/// </summary>
public interface IUpdater
{
    public void AssignTask(Action task);
}

public interface IUpdatable
{
    public void Update(IUpdater updater);
}

public class SimplyUpdater : IUpdater
{
    public void AssignTask(Action task)
    {
        task.Invoke();
    }
}
