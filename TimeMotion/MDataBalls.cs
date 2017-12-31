using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMotion
{
    class MDataBalls
    {
        //Constructor
        #region Constructor
        public MDataBalls (int id, bool fix = false)
        {
            BallID = id;
            FixedBall = fix;
        }
        #endregion

        //Public Data
        #region Public Data
        public int BallID
        {
            get;
            set;
        }
        public bool FixedBall
        {
            get;
            set;
        }
        #endregion
    }
}
