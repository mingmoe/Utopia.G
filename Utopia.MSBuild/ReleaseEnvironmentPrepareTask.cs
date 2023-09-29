using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utopia.MSBuild
{
    public class ReleaseEnvironmentPrepareTask : Task
    {

        public override bool Execute()
        {
            var pro = this.BuildEngine9.GetGlobalProperties();

// TODO

            return true;
        }
    }
}
