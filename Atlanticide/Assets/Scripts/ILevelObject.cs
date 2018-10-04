using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlanticide
{
    public interface ILevelObject
    {
        /// <summary>
        /// Resets the object to its default state.
        /// </summary>
        void ResetObject();
    }
}
