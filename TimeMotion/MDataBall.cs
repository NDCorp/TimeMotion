using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMotion
{
    class MDataBall
    {
        #region Constructor
        public MDataBall (int id)
        {
            BallID = id;
        }
        #endregion

        #region Public variable
        public int BallID { get; set; }
        #endregion
    }
}
